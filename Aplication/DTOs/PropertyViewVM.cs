using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class PropertyViewVM
    {
        [Required]
        public required int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string Location { get; set; }
        public decimal Price { get; set; }
        public int? Capacity { get; set; }
    }
}
