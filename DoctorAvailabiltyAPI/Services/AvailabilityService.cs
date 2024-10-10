using DoctorAvailabiltyAPI.Data;
using DoctorAvailabiltyAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DoctorAvailabiltyAPI.Services
{
    public class AvailabilityService
    {
        private readonly DbContextClass _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AvailabilityService(DbContextClass context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Get all availability for a doctor
        public async Task<List<DoctorAvailability>> GetDoctorAvailability(string doctorId)
        {
            var availabilities = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            // Handle potential null values in StartTime and EndTime
            foreach (var availability in availabilities)
            {
                availability.StartTime = availability.StartTime?.ToString() ?? "null"; // Use null-conditional operator
                availability.EndTime = availability.EndTime?.ToString() ?? "null"; // Use null-conditional operator
            }

            return availabilities;
        }



        // Get availability by ID
        public async Task<DoctorAvailability> GetDoctorAvailabilityById(int availabilityId)
        {
            return await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId);
        }

        // Get availability by doctor ID and date
        public async Task<IEnumerable<DoctorAvailability>> GetAvailabilityByDoctorIdAndDate(string doctorId, DateTime date)
        {
            return await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
                .ToListAsync();
        }

        // Check if doctor exists
        private async Task<bool> DoesDoctorExist(string doctorId)
        {
            var apiUrl = _configuration["DoctorAPI:BaseUrl"];
            var response = await _httpClient.GetAsync($"{apiUrl}/api/doctor/internal/getdoctorbyusername/{doctorId}");

            return response.IsSuccessStatusCode;
        }

        // Check for overlapping availability slots
        public async Task<bool> IsOverlapping(string doctorId, DateTime date, string startTimeStr, string endTimeStr)
        {
            // Parse the start and end times from string to TimeSpan
            if (!TimeSpan.TryParse(startTimeStr, out var startTime) || !TimeSpan.TryParse(endTimeStr, out var endTime))
            {
                throw new ArgumentException("Invalid time format.");
            }

            var availabilities = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
                .ToListAsync();

            return availabilities.Any(a =>
            {
                // Check if StartTime or EndTime are null or empty
                if (string.IsNullOrWhiteSpace(a.StartTime) || string.IsNullOrWhiteSpace(a.EndTime))
                {
                    return false; // Treat null or empty times as non-overlapping
                }

                // Try parsing the existing availability times
                if (!TimeSpan.TryParse(a.StartTime, out var existingStartTime) ||
                    !TimeSpan.TryParse(a.EndTime, out var existingEndTime))
                {
                    return false; // Treat improperly formatted times as non-overlapping
                }

                // Check for overlap
                return existingStartTime < endTime && existingEndTime > startTime;
            });
        }


        // Create availability
        public async Task<DoctorAvailability> CreateAvailability(DoctorAvailability availability)
        {
            // Check if the doctor exists
            if (!await DoesDoctorExist(availability.DoctorId))
            {
                throw new InvalidOperationException("The doctor does not exist.");
            }

            // If availability is not available (false), store the details and remove existing booked slots for the date
            if (!availability.IsAvailable)
            {
                // First, create a new record to store the unavailable details
                var unavailableAvailability = new DoctorAvailability
                {
                    DoctorId = availability.DoctorId,
                    Date = availability.Date,
                    StartTime = null, // Optional: You might want to set these to null or a specific value
                    EndTime = null,
                    IntervalMinutes = null,
                    IsAvailable = false,
                    Description = availability.Description // Store the description of unavailability
                };

                // Add the unavailable record to the context
                _context.DoctorAvailabilities.Add(unavailableAvailability);

                // Remove all booked slots for that date
                var bookedSlots = await _context.DoctorAvailabilities
                    .Where(a => a.DoctorId == availability.DoctorId && a.Date == availability.Date && a.IsAvailable)
                    .ToListAsync();

                // Remove the booked slots from the database
                _context.DoctorAvailabilities.RemoveRange(bookedSlots);
                await _context.SaveChangesAsync();

                // Return the new record or indicate the operation was successful
                await _context.SaveChangesAsync();
                return unavailableAvailability; // Return the newly created unavailable record
            }

            // Check for overlapping slots if the doctor is available
            var isOverlapping = await IsOverlapping(
                availability.DoctorId,
                availability.Date,
                availability.StartTime,  // Assuming these are strings in "HH:mm"
                availability.EndTime      // Assuming these are strings in "HH:mm"
            );

            if (isOverlapping)
            {
                throw new InvalidOperationException("The new availability slot overlaps with an existing slot.");
            }

            // Add the new availability record to the context
            _context.DoctorAvailabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        // Update availability
        public async Task<bool> UpdateAvailability(int availabilityId, DoctorAvailability updatedAvailability)
        {
            var existingAvailability = await GetDoctorAvailabilityById(availabilityId);
            if (existingAvailability == null)
            {
                return false;
            }

            // Only update fields if they are provided
            existingAvailability.Date = updatedAvailability.Date;
            existingAvailability.StartTime = updatedAvailability.StartTime;
            existingAvailability.EndTime = updatedAvailability.EndTime;
            existingAvailability.IntervalMinutes = updatedAvailability.IntervalMinutes;
            existingAvailability.IsAvailable = updatedAvailability.IsAvailable;

            _context.DoctorAvailabilities.Update(existingAvailability);
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete availability
        public async Task<bool> DeleteAvailability(int availabilityId)
        {
            var availability = await GetDoctorAvailabilityById(availabilityId);
            if (availability == null)
            {
                return false;
            }

            _context.DoctorAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
