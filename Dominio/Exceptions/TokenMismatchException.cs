using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class TokenMismatchException : BusinessRuleException
    {
        public TokenMismatchException(): base("Token does not match.")
        {
        }
    }
}
