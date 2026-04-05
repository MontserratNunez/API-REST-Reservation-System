using Aplication.DTOs.Authentication;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using Infrastructure.Persistence.Context;
using Aplication.Interfaces.IJwt;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthenticationController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("sing-in")]
        public async Task<IActionResult> Register([FromBody]RegisterVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Provide all required fields");
            }

            try
            {
                await _jwtService.Register(payload);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created(nameof(Register), $"User {payload.Email} created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Provide all required fields");
            }

            try
            {
                var tokenValue = await _jwtService.Login(payload);
                return Ok(tokenValue);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO payload)
        {
            try
            {
                var result = await _jwtService.VerifyAndGenerateTokenAsync(payload);

                if (result == null) { return BadRequest("Invalid tokens"); }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}
