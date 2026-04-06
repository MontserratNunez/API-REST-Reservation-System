using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserCreationFailedException : BusinessRuleException
    {
        public IReadOnlyCollection<string> Errors { get; }

        public UserCreationFailedException(IEnumerable<string> errors): base("User creation failed.")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}
