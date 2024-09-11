using Microsoft.EntityFrameworkCore;
using AdminAPI.Models;
using System.Collections.Generic;

namespace AdminAPI.Data
{
    public class DbContextClass : DbContext
    {
        public DbSet<Admin> Admins { get; set; }

        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }
    }
}
