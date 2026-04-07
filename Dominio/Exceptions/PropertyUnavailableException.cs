using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class PropertyUnavailableException : BusinessRuleException
    {
        public PropertyUnavailableException(): base("The property is no longer available for the selected dates.")
        {
        }
    }
}
