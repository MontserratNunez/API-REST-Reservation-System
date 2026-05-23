using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class GuestReservationVM : IReservationVM
    {
        public int Id { get; set; }
        public required string PropertyTitle { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int GuestQuantity { get; set; }
        public required bool HasReview { get; set; }
        public required ReservationStatus Status { get; set; }
    }
}
