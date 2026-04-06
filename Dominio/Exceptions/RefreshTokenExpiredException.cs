using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class RefreshTokenExpiredException : UnauthorizedDomainException
    {
        public RefreshTokenExpiredException()
            : base("Refresh token has expired. Please re-authenticate.")
        {
        }
    }
}
