using DoctorAvailabiltyAPI.Data;
using DoctorAvailabiltyAPI.Dtos;
using DoctorAvailabiltyAPI.Model;
using DoctorAvailabiltyAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoctorAvailabiltyAPI.Controllers
{
    [ApiController]
    [Route("api/availability")]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;

        public AvailabilityController(AvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        // Get all availability for a doctor
        [HttpGet("getdoctoravailability/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorAvailability>>> GetDoctorAvailability(string doctorId)
        {
            var availabilities = await _availabilityService.GetDoctorAvailability(doctorId);
            return Ok(availabilities);
        }

        // Get availability by ID
        [HttpGet("detail/{availabilityId}")]
        public async Task<ActionResult<DoctorAvailability>> GetAvailabilityById(int availabilityId)
        {
            var availability = await _availabilityService.GetDoctorAvailabilityById(availabilityId);
            if (availability == null) return NotFound();
            return Ok(availability);
        }

        // Get availability by doctor ID and date
        [HttpGet("getavailability/{doctorId}/{date}")]
        public async Task<ActionResult<IEnumerable<DoctorAvailability>>> GetAvailabilityByDoctorIdAndDate(string doctorId, DateTime date)
        {
            var availabilities = await _availabilityService.GetAvailabilityByDoctorIdAndDate(doctorId, date);
            if (availabilities == null || !availabilities.Any()) return NotFound();
            return Ok(availabilities);
        }

        // Create availability
        [HttpPost("createavailability")]
        public async Task<ActionResult<DoctorAvailability>> CreateAvailability([FromBody] AvailabilityDto dto)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine(subError.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            var availability = new DoctorAvailability
            {
                DoctorId = dto.DoctorId,
                Date = dto.Date,
                StartTime = dto.StartTime,  // Time as string
                EndTime = dto.EndTime,      // Time as string
                IntervalMinutes = dto.IntervalMinutes ??0,
                IsAvailable = dto.IsAvailable,
                Description = dto.Description,
            };

            try
            {
                var createdAvailability = await _availabilityService.CreateAvailability(availability);
                if (createdAvailability == null) // Indicating all slots were cleared
                {
                    return NoContent(); // 204 No Content
                }
                return CreatedAtAction(nameof(GetAvailabilityById), new { availabilityId = createdAvailability.AvailabilityId }, createdAvailability);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 Conflict
            }
        }

        // Update availability
        [HttpPut("updateavailability/{availabilityId}")]
        public async Task<IActionResult> UpdateAvailability(int availabilityId, [FromBody] AvailabilityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var availability = new DoctorAvailability
            {
                DoctorId = dto.DoctorId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IntervalMinutes = dto.IntervalMinutes,
                IsAvailable = dto.IsAvailable
            };

            var success = await _availabilityService.UpdateAvailability(availabilityId, availability);
            if (!success) return NotFound("Availability not found.");
            return NoContent();
        }

        // Delete availability
        [HttpDelete("deleteavailability/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            var success = await _availabilityService.DeleteAvailability(availabilityId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
