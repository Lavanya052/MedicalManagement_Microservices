using Microsoft.EntityFrameworkCore;
using AvailabilityAPI.Model;

namespace AvailabilityAPI.Data
{
    public class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }

        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Slot> Slots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorAvailability>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Availability>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Slot>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<DoctorAvailability>()
                .HasMany(d => d.AvailabilityData)
                .WithOne(a => a.DoctorAvailability)
                .HasForeignKey(a => a.DoctorAvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Availability>()
                .HasMany(a => a.Slots)
                .WithOne(s => s.Availability)
                .HasForeignKey(s => s.AvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
