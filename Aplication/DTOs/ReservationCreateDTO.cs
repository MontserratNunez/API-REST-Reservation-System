using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class ReservationCreateDTO
    {
        [Required(ErrorMessage = "You must enter the start date.")]
        public required DateTime StartDate { get; set; }
        [Required(ErrorMessage = "You must enter the end date.")]
        public required DateTime EndDate { get; set; }
        [Required(ErrorMessage = "You must enter the ammount of guests.")]
        public required int GuestQuantity { get; set; }
    }
}
