using AvailabilityAPI.Data;
using AvailabilityAPI.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AvailabilityAPI.Service
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly DbContextClass _context;

        public AvailabilityService(DbContextClass context)
        {
            _context = context;
        }

        public async Task AddDoctorAvailability(DoctorAvailability doctorAvailability)
        {
            var existingDoctor = await _context.DoctorAvailabilities
                .Include(d => d.AvailabilityData)
                .ThenInclude(a => a.Slots)
                .SingleOrDefaultAsync(d => d.DoctorId == doctorAvailability.DoctorId);

            if (existingDoctor != null)
            {
                // Update existing doctor availability
                existingDoctor.DoctorName = doctorAvailability.DoctorName;
                existingDoctor.AvailabilityData = doctorAvailability.AvailabilityData;
                _context.DoctorAvailabilities.Update(existingDoctor);
            }
            else
            {
                // Add new doctor availability
                _context.DoctorAvailabilities.Add(doctorAvailability);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<DoctorAvailability> GetDoctorAvailability(string doctorId)
        {
            return await _context.DoctorAvailabilities
                .Include(d => d.AvailabilityData)
                .ThenInclude(a => a.Slots)
                .SingleOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task UpdateDoctorAvailability(DoctorAvailability doctorAvailability)
        {
            var existingDoctor = await _context.DoctorAvailabilities
                .Include(d => d.AvailabilityData)
                .ThenInclude(a => a.Slots)
                .SingleOrDefaultAsync(d => d.Id == doctorAvailability.Id);

            if (existingDoctor == null) throw new Exception("Doctor availability not found.");

            existingDoctor.DoctorName = doctorAvailability.DoctorName;
            existingDoctor.AvailabilityData = doctorAvailability.AvailabilityData;

            _context.DoctorAvailabilities.Update(existingDoctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAvailability(int id)
        {
            var doctorAvailability = await _context.DoctorAvailabilities.FindAsync(id);
            if (doctorAvailability == null) throw new Exception("Doctor availability not found.");

            _context.DoctorAvailabilities.Remove(doctorAvailability);
            await _context.SaveChangesAsync();
        }
    }
}
