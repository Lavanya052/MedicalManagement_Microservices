using AvailabilityAPI.Model;
using AvailabilityAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvailabilityAPI.Controllers
{
    [Route("api/doctoravailability")]
    [ApiController]
    public class DoctorAvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _repository;

        public DoctorAvailabilityController(IAvailabilityService repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctorAvailability([FromBody] DoctorAvailability doctorAvailability)
        {
            try
            {
                await _repository.AddDoctorAvailability(doctorAvailability);
                return Ok(new { message = "Doctor availability added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{doctorId}")]
        public async Task<ActionResult<DoctorAvailability>> GetDoctorAvailability(string doctorId)
        {
            var doctorAvailability = await _repository.GetDoctorAvailability(doctorId);
            if (doctorAvailability == null) return NotFound();
            return Ok(doctorAvailability);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorAvailability(int id, [FromBody] DoctorAvailability doctorAvailability)
        {
            if (id != doctorAvailability.Id) return BadRequest();

            try
            {
                await _repository.UpdateDoctorAvailability(doctorAvailability);
                return Ok(new { message = "Doctor availability updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorAvailability(int id)
        {
            try
            {
                await _repository.DeleteDoctorAvailability(id);
                return Ok(new { message = "Doctor availability deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
