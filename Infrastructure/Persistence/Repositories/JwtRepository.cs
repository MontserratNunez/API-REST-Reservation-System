using Aplication.DTOs.Authentication;
using Aplication.Interfaces.IJwt;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using my_books.Data.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class JwtRepository : IJwtRepository
    {
        private readonly AppDbContext _context;

        public JwtRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveJwtToken(RefreshToken refreshToken)
        {  
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> RefreshTokens(TokenRequestDTO payload)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);
        }
    }
}
