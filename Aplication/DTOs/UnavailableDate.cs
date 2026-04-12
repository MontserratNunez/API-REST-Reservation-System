using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class UnavailableDate
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
