using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class TokenNotExpiredException : BusinessRuleException
    {
        public TokenNotExpiredException()
            : base("Token has not expired yet.")
        {
        }
    }
}
