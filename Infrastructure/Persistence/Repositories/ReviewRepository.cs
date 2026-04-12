using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aplication.Interfaces;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Review> dbSet;

        public ReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<Review>();
        }

        public async Task<List<Review>> GetByProperty(int propertyId)
        {
            return await dbSet.Where(r => r.IdProperty == propertyId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }
    }
}