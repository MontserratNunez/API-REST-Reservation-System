using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class PropertyUpdateDTO
    {
        public int Id { get; set; }
        public int IdHost { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? Price { get; set; }
        public int? Capacity { get; set; }
    }

}
