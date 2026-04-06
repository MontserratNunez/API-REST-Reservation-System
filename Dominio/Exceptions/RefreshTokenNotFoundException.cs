using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class RefreshTokenNotFoundException : ResourceNotFoundException
    {
        public RefreshTokenNotFoundException(): base("Refresh token does not exist.")
        {
        }
    }
}
