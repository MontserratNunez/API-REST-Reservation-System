using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetByPropertyIdAsync(int propertyId);

        Task<bool> OverlappingReservation(int propertyId, DateTime startDate, DateTime endDate);
    }

}
