using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTOs
{
    public class HostReservationVM : IReservationVM
    {
        public required string PropertyTitle { get; set; }
        public required string GuestName { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int GuestQuantity { get; set; }
        public required ReservationStatus Status { get; set; }
    }
}
