using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class RefreshTokenRevokedException : UnauthorizedDomainException
    {
        public RefreshTokenRevokedException(): base("Refresh token has been revoked.")
        {
        }
    }
}
