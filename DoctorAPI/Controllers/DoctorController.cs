using DoctorAPI.Services;
using DoctorAPI.Models;
using DoctorAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DoctorAPI.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(DoctorService doctorService, ILogger<DoctorController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        // Get all doctors
        [HttpGet("getalldoctor")]
        public async Task<IEnumerable<DoctorDto>> GetDoctors()
        {
            return await _doctorService.GetAllDoctors();
        }

        // Get doctor by ID
        [HttpGet("getdoctorbyid/{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorById([FromRoute] string id)
        {
            var doctor = await _doctorService.GetDoctorById(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpPost("createdoctor")]
        public async Task<ActionResult<DoctorDto>> CreateDoctor([FromBody] DoctorInternalDto doctorInternalDto)
        {
            // Log incoming data for debugging
            _logger.LogInformation("Received DoctorInternalDto: {@DoctorInternalDto}", doctorInternalDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctor = new Doctor
            {
                Name = doctorInternalDto.Name,
                Specialization = doctorInternalDto.Specialization,
                Email = doctorInternalDto.Email,
                PhoneNumber = doctorInternalDto.PhoneNumber,
                ImageBase64 = doctorInternalDto.ImageBase64,
                Password = doctorInternalDto.Password,
                Username =doctorInternalDto.Username,
                Gender =doctorInternalDto.Gender,
                Address=doctorInternalDto.Address,
                DOB=doctorInternalDto.DOB,
            };

            var createdDoctor = await _doctorService.CreateDoctor(doctor);

            var createdDoctorDto = new DoctorDto
            {
                DoctorId = createdDoctor.DoctorId,
                Name = createdDoctor.Name,
                Specialization = createdDoctor.Specialization,
                Email = createdDoctor.Email,
                PhoneNumber = createdDoctor.PhoneNumber,
                ImageBase64 = createdDoctor.ImageBase64
            };

            return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.DoctorId }, createdDoctorDto);
        }


        // Update doctor
        [HttpPut("updatedoctor/{id}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] string id, [FromBody] DoctorDto doctorDto)
        {
            var doctor = new Doctor
            {
                Name = doctorDto.Name,
                Specialization = doctorDto.Specialization,
                Email = doctorDto.Email,
                PhoneNumber = doctorDto.PhoneNumber,
                ImageBase64 = doctorDto.ImageBase64 // Use ImageBase64 for update
            };

            var updatedDoctor = await _doctorService.UpdateDoctor(id, doctor);
            if (updatedDoctor == null) return NotFound();
            return NoContent();
        }

        // Delete doctor
        [HttpDelete("deletedoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] string id)
        {
            var success = await _doctorService.DeleteDoctor(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("getspecializations")]
        public async Task<IActionResult> GetSpecializations()
        {
            var specializations = await _doctorService.GetDistinctSpecializations();
            return Ok(specializations);
        }

    }
}
