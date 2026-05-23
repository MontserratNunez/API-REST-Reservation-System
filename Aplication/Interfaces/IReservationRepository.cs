using Domain.Entity;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetByPropertyIdAsync(int propertyId);
        Task<bool> OverlappingReservation(int propertyId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<int>> GetOverlappingPropertyIds(DateTime startDate,DateTime endDate);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);

        Task<IEnumerable<Reservation>> CancelReservations(int propertyId);
    }
}
