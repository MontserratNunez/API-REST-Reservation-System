using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs.Review
{
    public class PropertyRatingVM
    {
        public int PropertyId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}