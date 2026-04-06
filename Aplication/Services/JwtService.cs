using Aplication.DTOs.Authentication;
using Aplication.Emails;
using Aplication.Interfaces;
using Aplication.Interfaces.IJwt;
using Domain.Entity;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using my_books.Data.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtRepository _jwtRepository;
        private readonly IEmailService _emailService;

        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtService(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration, 
            IJwtRepository jwtRepository, 
            TokenValidationParameters tokenValidationParameters,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _jwtRepository = jwtRepository;
            _tokenValidationParameters = tokenValidationParameters;

            _emailService = emailService;
        }

        public async Task Register([FromBody] RegisterVM payload)
        {
            var userExists = await _userManager.FindByEmailAsync(payload.Email);

            if (userExists != null)
            {
                throw new UserAlreadyExistsException(payload.Email);
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = payload.Email,
                UserName = payload.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, payload.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                throw new UserCreationFailedException(errors);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var param = new Dictionary<string, string?>
            {
                {"token", token},
                {"email", newUser.Email }
            };

            var callback = QueryHelpers.AddQueryString(payload.ClientUri!, param);

            var body = EmailTemplates.ConfirmAccount(callback);

            _ = Task.Run(async () =>
            {
                await _emailService.SendEmail(
                    payload.Email,
                    "Confirm your account",
                    body
                );
            });

            switch (payload.Role)
            {
                case "Admin":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin.ToString());
                    break;
                case "Host":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Host.ToString());
                    break;
                default:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Guest.ToString());
                    break;
            }
        }

        public async Task<AuthResultVM> Login([FromBody] LoginVM payload)
        {
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new UnauthorizedDomainException("Email is not confirmed");

            if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                var tokenValue = await GenerateJwtTokenAsync(user, "");

                return tokenValue;
            }
            throw new InvalidCredentialsException();
        }

        private async Task<AuthResultVM> GenerateJwtTokenAsync(ApplicationUser user, string existingRefreshToken)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Add User Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10), // 5 - 10mins
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };

                await _jwtRepository.SaveJwtToken(refreshToken);
            }

            var response = new AuthResultVM()
            {
                Token = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(existingRefreshToken)) ? refreshToken.Token : existingRefreshToken,
                ExpiresAt = token.ValidTo
            };

            return response;
        }

        public async Task<AuthResultVM> VerifyAndGenerateTokenAsync(TokenRequestDTO payload)
        {
            var jwTokenHandler = new JwtSecurityTokenHandler();

            var tokenInVerification = jwTokenHandler.ValidateToken(payload.Token, _tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (result == false) return null;
            }

            var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDate = UnixTimeStampToDateTimeInUTC(utcExpiryDate);
            if (expiryDate > DateTime.UtcNow) throw new TokenNotExpiredException();

            var dbRefreshToken = await _jwtRepository.RefreshTokens(payload);

            if (dbRefreshToken == null)
            {
                throw new RefreshTokenNotFoundException();
            }
            else
            {
                //Check 5 - Validate Id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (dbRefreshToken.JwtId != jti) throw new TokenMismatchException();

                //Check 6 - Refresh token expiration
                if (dbRefreshToken.DateExpire <= DateTime.UtcNow) throw new RefreshTokenExpiredException();

                //Check 7 - Refresh token Revoked
                if (dbRefreshToken.IsRevoked) throw new RefreshTokenRevokedException();

                //Generate new token (with existing refresh token)
                var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                return await newTokenResponse;
            }
        }

        private DateTime UnixTimeStampToDateTimeInUTC(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal;
        }

        public async Task EmailConfirmation(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                throw new BusinessRuleException("Invalid email confirmation request");
            }

            var confirmResult  = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirmResult.Succeeded)
            {
                throw new BusinessRuleException("Invalid email confirmation request");
            }
        }
    }
}
