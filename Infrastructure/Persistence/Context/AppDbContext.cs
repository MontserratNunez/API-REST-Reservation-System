using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> db) : base(db){ }
        public DbSet<Property> Property { get; set; }
        /*public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Lock> Lock { get; set; }
        public DbSet<Notification> Notification { get; set; }*/
    }
}