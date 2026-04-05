using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Reservation
    {
        public int Id { get; set; }
        public required int IdProperty { get; set; }
        public required string IdGuest { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int GuestQuantity { get; set; }
        public required ReservationStatus Status { get; set; }
    }
}
