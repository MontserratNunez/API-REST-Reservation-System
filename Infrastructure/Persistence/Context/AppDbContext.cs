using Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> db) : base(db){ }
        public DbSet<Property> Property { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Lock> Lock { get; set; }
        //public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}