using Microsoft.EntityFrameworkCore;
using PatientAPI.Model;
using System.Collections.Generic;

namespace PatientAPI.Data
{
    public class DbContextClass : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }
    }
}
