using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class ReviewCreateDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
