using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class InvalidCredentialsException : UnauthorizedDomainException
    {
        public InvalidCredentialsException(): base("Invalid email or password.")
        {
        }
    }
}
