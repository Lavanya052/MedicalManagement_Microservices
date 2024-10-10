using Microsoft.EntityFrameworkCore;
using DoctorAvailabiltyAPI.Model;
using System;

namespace DoctorAvailabiltyAPI.Data
{
    public class DbContextClass : DbContext
    {
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DoctorAvailability>()
                .HasKey(da => da.AvailabilityId); // Define primary key

            modelBuilder.Entity<DoctorAvailability>()
       .Property(da => da.Date)
       .HasColumnType("date"); // Use SQL 'date' type
        }

    }
}
