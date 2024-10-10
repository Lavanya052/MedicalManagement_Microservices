using AppointmentAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAPI.Data
{
    public class DbContextClass: DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options)
        {
        }

        // DbSet for Doctors
        public DbSet<BookedAppointment> BookedAppointments { get; set; }

        // DbSet for Appointments
        public DbSet<Appointment> Appointments { get; set; }

    }
}
