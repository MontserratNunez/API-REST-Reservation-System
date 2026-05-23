using Aplication.Interfaces;
using Domain.Entity;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<IEnumerable<int>> GetOverlappingPropertyIds(DateTime startDate, DateTime endDate)
        {
            return await dbSet.Where(r =>
                r.Status == ReservationStatus.Confirmed &&
                startDate < r.EndDate &&
                endDate > r.StartDate)
                .Select(r => r.IdProperty)
                .Distinct()
                .ToListAsync();
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task<IEnumerable<Reservation>> CancelReservations(int propertyId)
        {
            var reservations = await dbSet.Where(r => r.IdProperty == propertyId && r.Status == ReservationStatus.Confirmed).ToListAsync();

            foreach (var reservation in reservations)
            {
                reservation.Status = ReservationStatus.Canceled;
            }

            await _context.SaveChangesAsync();

            return reservations;
        }
    }
}
