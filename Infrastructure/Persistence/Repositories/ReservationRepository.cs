using Aplication.Interfaces;
using Domain.Entity;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ReservationRepository: Repository<Reservation>, IReservationRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Reservation> dbSet;

        public ReservationRepository(AppDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<Reservation>();
        }

        public async Task<IEnumerable<Reservation>> GetByPropertyIdAsync(int propertyId)
        {
            return await dbSet.Where(r => r.IdProperty == propertyId).ToListAsync();
        }

        public async Task<bool> OverlappingReservation(
            int propertyId,
            DateTime startDate,
            DateTime endDate)
        {
            return await dbSet.AnyAsync(r =>
                r.IdProperty == propertyId &&
                r.Status == ReservationStatus.Confirmed &&
                startDate < r.EndDate &&
                endDate > r.StartDate
            );
        }
    }
}
