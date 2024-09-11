using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Log incoming request
            _logger.LogInformation("Received login request: {@LoginRequest}", loginRequest);

            // Fetch admin details
            var admin = await _authService.GetAdminDetails(loginRequest.Id);

            // Check if admin exists and passwords match
            if (admin != null && VerifyPassword(loginRequest.Password, admin.Password))
            {
                var token = _authService.GenerateJwtToken(admin.Email);
                return Ok(new { Token = token });
            }

            var doctor = await _authService.GetDoctorDetails(loginRequest.Id);

            // Check if admin exists and passwords match
            if (doctor != null && VerifyPassword(loginRequest.Password, doctor.Password))
            {
                var token = _authService.GenerateJwtToken(doctor.Email);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private bool VerifyPassword(string providedPassword, string storedPassword)
        {
            return providedPassword == storedPassword;
        }

        public class LoginRequest
        {
            public string Id { get; set; }
            public string Password { get; set; }
        }
    }
}
