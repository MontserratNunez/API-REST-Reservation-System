using Aplication.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            dbSet = context.Set<T>();
        }

        public async Task Add(T entity)
        {
            dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await GetValue(id);
            if (entity == null)
            {
                throw new KeyNotFoundException();
            }
            
            dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetValue(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task Update(T entity)
        {
            dbSet.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
