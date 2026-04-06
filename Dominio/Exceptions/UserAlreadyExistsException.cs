using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserAlreadyExistsException : BusinessRuleException
    {
        public UserAlreadyExistsException(string email)
            : base($"User with email '{email}' already exists.")
        {
        }
    }
}
