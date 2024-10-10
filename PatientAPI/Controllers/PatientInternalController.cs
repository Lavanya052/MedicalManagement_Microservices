using PatientAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PatientAPI.Controllers
{
    [Route("api/patient/internal")]
    [ApiController]
    public class PatientInternalController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientInternalController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("getpatientbyusername/{id}")]
        public async Task<IActionResult> GetPatientByUsername([FromRoute] string id)
        {
            var patient = await _patientService.GetPatientByUsername(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }
    }
}
