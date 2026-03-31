using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs.Authentication
{
    public class TokenRequestDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
