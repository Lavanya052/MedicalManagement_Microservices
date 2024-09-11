using Microsoft.EntityFrameworkCore;
using DoctorAPI.Models;
using System.Collections.Generic;

namespace DoctorAPI.Data
{
    public class DbContextClass : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }

        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }
    }
}
