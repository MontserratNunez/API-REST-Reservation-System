using Aplication.DTOs.Authentication;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IJwtRepository
    {
        Task SaveJwtToken(RefreshToken refreshToken);
        Task<RefreshToken?> RefreshTokens(TokenRequestDTO payload);
    }
}
