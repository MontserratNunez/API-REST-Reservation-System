using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Property
    {
        public int Id { get; set; }
        [Required]
        public required string IdHost { get; set; }

        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public required string Location { get; set; }
        [Required]
        public required decimal Price { get; set; }
        public int? Capacity { get; set; }
    }
}
