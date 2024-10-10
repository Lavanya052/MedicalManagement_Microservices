using DoctorAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAPI.Controllers
{
    [Route("api/doctor/internal")]
    [ApiController]
    public class DoctorInternalController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorInternalController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("getdoctorbyusername/{id}")]
        public async Task<IActionResult> GetDoctorByUsername([FromRoute]string id)
        {
            var doctor = await _doctorService.GetDoctorByUsername(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }
    }

}
