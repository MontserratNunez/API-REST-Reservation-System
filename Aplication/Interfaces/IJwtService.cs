using Aplication.DTOs.Authentication;
using my_books.Data.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IJwtService
    {
        Task Register(RegisterVM payload);
        Task<AuthResultVM> Login(LoginVM payload);
        Task<AuthResultVM> VerifyAndGenerateTokenAsync(TokenRequestDTO payload);
    }
}
