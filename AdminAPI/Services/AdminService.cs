using AdminAPI.Models;
using AdminAPI.Dtos;
using AdminAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
namespace AdminAPI.Services
{
    public class AdminService
    {
        private readonly DbContextClass _dbContext;

        public AdminService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all admins
        public async Task<IEnumerable<AdminDto>> GetAllAdmins()
        {
            return await _dbContext.Admins
                .Select(admin => new AdminDto
                {
                    Id = admin.Id,
                    Username = admin.Username,
                    Email = admin.Email,
                    Name=admin.Name,   
                    Phone=admin.Phone,
                    
                })
                .ToListAsync();
        }

        // Get admin by ID
        public async Task<AdminDto> GetAdminById(string id)
        {
            var admin = await _dbContext.Admins.FindAsync(id);
            if (admin == null) return null;

            return new AdminDto
            {
                Id = admin.Id,
                Username = admin.Username,
                Email = admin.Email,
                Name = admin.Name,
                Phone = admin.Phone,
            };
        }
        //to pass to interval values

        public async Task<AdminInternalDto> GetAdminByUsername(string IdorEmail)
        {
            var admin = await _dbContext.Admins
                .Where(a => a.Id == IdorEmail || a.Email == IdorEmail)
                .Select(a => new AdminInternalDto
                {
                    Id = a.Id,
                    Username = a.Username,
                    Email = a.Email,
                    Role = a.Role,
                    Password = a.Password,
                    Name = a.Name,
                    Phone = a.Phone,
                })
                .FirstOrDefaultAsync();

            return admin;
        }

        // Create a new admin
        public async Task<Admin> CreateAdmin(Admin admin)
        {
            try
            {
                admin.Role = "admin";

                // Generate a new unique ID
                var newId = GenerateNewId();
                while (await _dbContext.Admins.AnyAsync(a => a.Id == newId))
                {
                    newId = GenerateNewId();
                }
                admin.Id = newId;

                // Hash the password before storing it
                admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);

                _dbContext.Admins.Add(admin);
                await _dbContext.SaveChangesAsync();
                return admin;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new InvalidOperationException("Error creating admin.", ex);
            }
        }

        // Update an existing admin
        public async Task<Admin> UpdateAdmin(string id, Admin updatedAdmin)
        {
            var admin = await _dbContext.Admins.FindAsync(id);
            if (admin == null) return null;

            admin.Username = updatedAdmin.Username;
            admin.Email = updatedAdmin.Email;
            admin.Password = updatedAdmin.Password;
            await _dbContext.SaveChangesAsync();
            return admin;
        }

        // Delete an admin
        public async Task<bool> DeleteAdmin(string id)
        {
            var admin = await _dbContext.Admins.FindAsync(id);
            if (admin == null) return false;

            _dbContext.Admins.Remove(admin);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Generate a new unique ID
        private string GenerateNewId()
        {
            // Assuming the ID format is "A001", "A002", etc.
            var lastAdmin = _dbContext.Admins
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (lastAdmin == null)
            {
                return "A001";
            }

            try
            {
                var lastIdNumber = int.Parse(lastAdmin.Id.Substring(1));
                var newIdNumber = lastIdNumber + 1;
                return $"A{newIdNumber:D3}";
            }
            catch (FormatException ex)
            {
                // Handle format exception if ID format is not as expected
                throw new Exception("Error while parsing the last admin ID", ex);
            }
        }
    }
}
