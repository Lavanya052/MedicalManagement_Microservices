using AuthAPI.Models;
using AuthAPI.Services;
using BCrypt.Net;
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
                var loginResponse = new LoginResponse
                {
                    Token = token,
                    Username = admin.Username,
                    Email = admin.Email,
                    Role = "admin",
                    ImageBase64 = null,
                    Id=admin.Id,
                };
                return Ok(loginResponse);
            }

            // Fetch doctor details
            var doctor = await _authService.GetDoctorDetails(loginRequest.Id);

            // Check if doctor exists and passwords match
            if (doctor != null && VerifyPassword(loginRequest.Password, doctor.Password))
            {
                var token = _authService.GenerateJwtToken(doctor.Email);
                var loginResponse = new LoginResponse
                {
                    Token = token,
                    Username = doctor.Name,
                    Email = doctor.Email,
                    Role = "doctor",
                    ImageBase64 = doctor.ImageBase64,
                    Id=doctor.DoctorId,
                };
                return Ok(loginResponse);
            }

            return Unauthorized();
        }

        private bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            // Verify the provided password against the stored hashed z
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            _logger.LogInformation("Received forgot password request for: {Email}", forgotPasswordRequest.Email);
            var token = await _authService.GeneratePasswordResetToken(forgotPasswordRequest.Email);

            if (token != null)
            {
                return Ok(new { Message = "Password reset link has been sent to your email." });
            }

            return BadRequest("Email not found.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            _logger.LogInformation("Received password reset request for token: {Token}", resetPasswordRequest.Token);

            // Validate the token (assuming you have a method for this)
            var tokenValid = _authService.ValidateToken(resetPasswordRequest.Token, resetPasswordRequest.Email);
            if (!tokenValid)
            {
                return BadRequest("Invalid or expired token.");
            }

            var success = await _authService.ResetPassword(resetPasswordRequest.Email, resetPasswordRequest.NewPassword);
            if (success)
            {
                return Ok(new { Message = "Password has been reset successfully." });
            }

            return BadRequest("Password reset failed.");
        }



        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Token { get; set; }
            public string Email { get; set; }
            public string NewPassword { get; set; }
        }


        public class LoginRequest
        {
            public string Id { get; set; }
            public string Password { get; set; }
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }

            public string Id { get; set; }
            public string? ImageBase64 { get; set; }
        }
    }
}
