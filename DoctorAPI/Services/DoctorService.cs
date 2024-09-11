using DoctorAPI.Models;
using DoctorAPI.Dtos;
using DoctorAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace DoctorAPI.Services
{
    public class DoctorService
    {
        private readonly DbContextClass _dbContext;

        public DoctorService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all doctors
        public async Task<IEnumerable<DoctorDto>> GetAllDoctors()
        {
            return await _dbContext.Doctors
                .Select(doctor => new DoctorDto
                {
                    Id = doctor.Id,
                    Name = doctor.Name,
                    Specialization = doctor.Specialization,
                    Email = doctor.Email,
                    PhoneNumber = doctor.PhoneNumber
                })
                .ToListAsync();
        }

        // Get doctor by ID
        public async Task<DoctorDto> GetDoctorById(string id)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor == null) return null;

            return new DoctorDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber
            };
        }
        public async Task<DoctorInternalDto> GetDoctorByUsername(string Id)
        {
            var doctor = await _dbContext.Doctors
                .Where(d => d.Id == Id)
                .Select(d => new DoctorInternalDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Specialization = d.Specialization,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    Password = d.Password
                })
                .FirstOrDefaultAsync();

            return doctor;
        }
        // Create a new doctor
        public async Task<Doctor> CreateDoctor(Doctor doctor)
        {
            try
            {
                // Generate a new unique ID
                var newId = GenerateNewId();
                while (await _dbContext.Doctors.AnyAsync(d => d.Id == newId))
                {
                    newId = GenerateNewId();
                }
                doctor.Id = newId;

                _dbContext.Doctors.Add(doctor);
                await _dbContext.SaveChangesAsync();
                return doctor;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error while creating doctor", ex);
            }
        }

        // Update an existing doctor
        public async Task<Doctor> UpdateDoctor(string id, Doctor updatedDoctor)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor == null) return null;

            doctor.Name = updatedDoctor.Name;
            doctor.Specialization = updatedDoctor.Specialization;
            doctor.Email = updatedDoctor.Email;
            doctor.PhoneNumber = updatedDoctor.PhoneNumber;
            await _dbContext.SaveChangesAsync();
            return doctor;
        }

        // Delete a doctor
        public async Task<bool> DeleteDoctor(string id)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor == null) return false;

            _dbContext.Doctors.Remove(doctor);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Generate a new unique ID
        private string GenerateNewId()
        {
            var lastDoctor = _dbContext.Doctors
                .OrderByDescending(d => d.Id)
                .FirstOrDefault();

            if (lastDoctor == null)
            {
                return "D001";
            }

            try
            {
                var lastIdNumber = int.Parse(lastDoctor.Id.Substring(1));
                var newIdNumber = lastIdNumber + 1;
                return $"D{newIdNumber:D3}";
            }
            catch (FormatException ex)
            {
                throw new Exception("Error while parsing the last doctor ID", ex);
            }
        }
    }
}
