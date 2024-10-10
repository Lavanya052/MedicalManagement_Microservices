using PatientAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientAPI.Dto;
using PatientAPI.Model;

namespace PatientAPI.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(PatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        // Get all patients
        [HttpGet("getallpatients")]
        public async Task<IEnumerable<PatientDto>> GetPatients()
        {
            return await _patientService.GetAllPatients();
        }

        // Get patient by ID
        [HttpGet("getpatientbyid/{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById([FromRoute] string id)
        {
            var patient = await _patientService.GetPatientById(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        // Create a new patient
        [HttpPost("createpatient")]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] PatientInternalDto patientInternalDto)
        {
            _logger.LogInformation("Received PatientInternalDto: {@PatientInternalDto}", patientInternalDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = new Patient
            {
                Name = patientInternalDto.Name,
                BloodGroup = patientInternalDto.BloodGroup,
                Email = patientInternalDto.Email,
                PhoneNumber = patientInternalDto.PhoneNumber,
                ImageBase64 = patientInternalDto.ImageBase64,
                Password = patientInternalDto.Password,
                DOB = patientInternalDto.DOB,
                Gender = patientInternalDto.Gender,
                Address = patientInternalDto.Address
            };

            var createdPatient = await _patientService.CreatePatient(patient);

            var createdPatientDto = new PatientDto
            {
                PatientId = createdPatient.PatientId,
                Name = createdPatient.Name,
                BloodGroup = createdPatient.BloodGroup,
                Email = createdPatient.Email,
                PhoneNumber = createdPatient.PhoneNumber,
                ImageBase64 = createdPatient.ImageBase64
            };

            return Ok(createdPatientDto);
        }

        // Update patient
        [HttpPut("updatepatient/{id}")]
        public async Task<ActionResult> UpdatePatient([FromRoute] string id, [FromBody] PatientDto patientInternalDto)
        {
            var patient = new Patient
            {
                Name = patientInternalDto.Name,
                BloodGroup = patientInternalDto.BloodGroup,
                Email = patientInternalDto.Email,
                PhoneNumber = patientInternalDto.PhoneNumber,
                ImageBase64 = patientInternalDto.ImageBase64
            };

            var updatedPatient = await _patientService.UpdatePatient(id, patient);
            if (updatedPatient == null) return NotFound();
            return NoContent();
        }

        // Delete patient
        [HttpDelete("deletepatient/{id}")]
        public async Task<ActionResult> DeletePatient([FromRoute] string id)
        {
            var result = await _patientService.DeletePatient(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
