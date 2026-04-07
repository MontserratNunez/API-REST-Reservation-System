using Aplication.Interfaces;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class LockRepository: Repository<Lock>, ILockRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Lock> dbSet;

        public LockRepository(AppDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<Lock>();
        }

        public async Task<bool> OverlappingBlock(
            int propertyId,
            DateTime startDate,
            DateTime endDate)
        {
            return await dbSet.AnyAsync(l =>
                l.IdProperty == propertyId &&
                startDate < l.EndDate &&
                endDate > l.StartDate
            );
        }
        public async Task<IEnumerable<int>> GetOverlappingPropertyIds(DateTime startDate, DateTime endDate)
        {
            return await dbSet.Where(l =>
                startDate < l.EndDate &&
                endDate > l.StartDate)
                .Select(l => l.IdProperty)
                .Distinct()
                .ToListAsync();
        }

    }
}
