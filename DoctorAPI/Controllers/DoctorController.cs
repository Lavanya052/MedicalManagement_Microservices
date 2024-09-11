using DoctorAPI.Services;
using DoctorAPI.Models;
using DoctorAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoctorAPI.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // Get all doctors
        [HttpGet]
        public async Task<IEnumerable<DoctorDto>> GetDoctors()
        {
            return await _doctorService.GetAllDoctors();
        }

        // Get doctor by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorById(string id)
        {
            var doctor = await _doctorService.GetDoctorById(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        // Create a new doctor
        [HttpPost]
        public async Task<ActionResult<Doctor>> CreateDoctor(Doctor doctor)
        {
            var createdDoctor = await _doctorService.CreateDoctor(doctor);
            return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Id }, createdDoctor);
        }

        // Update doctor
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, Doctor doctor)
        {
            var updatedDoctor = await _doctorService.UpdateDoctor(id, doctor);
            if (updatedDoctor == null) return NotFound();
            return NoContent();
        }

        // Delete doctor
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var success = await _doctorService.DeleteDoctor(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
