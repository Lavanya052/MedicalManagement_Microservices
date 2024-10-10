using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;


namespace AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<AdminInternalDto> GetAdminDetails(string username)
        {
            var apiUrl = _configuration["AdminAPI:BaseUrl"];
            var response = await _httpClient.GetAsync($"{apiUrl}/api/admin/internal/getadminbyusername/{username}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AdminInternalDto>();
            }

            return null;
        }

        public async Task<DoctorInternalDto> GetDoctorDetails(string username)
        {
            var apiUrl = _configuration["DoctorAPI:BaseUrl"];
            var response = await _httpClient.GetAsync($"{apiUrl}/api/doctor/internal/getdoctorbyusername/{username}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DoctorInternalDto>();
            }

            return null;
        }

        public string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GeneratePasswordResetToken(string email)
        {
            var admin = await GetAdminDetails(email);
            if (admin != null)
            {
                var token = GenerateJwtToken(email); // Can be a temporary token, you can customize it
                await SendResetEmail(admin.Email, token,admin.Name);
                return token;
            }

            var doctor = await GetDoctorDetails(email);
            if (doctor != null)
            {
                var token = GenerateJwtToken(email); // Same token method used here
                await SendResetEmail(doctor.Email, token,doctor.Name);
                return token;
            }

            return null;
        }

        private async Task SendResetEmail(string email, string token, string Name)
        {
            var resetLink = $"http://localhost:4200/reset-password?token={token}";
            var subject = "Password Reset Request";

            // Personalized body content with Name and reset link
            var body = $@"
        <p>Hello {Name},</p>
        <p>We received a request to reset your password. You can reset it by clicking the link below:</p>
        <p><a href='{resetLink}'>Click here to reset your password</a></p>
        <p>If you didn't request a password reset, please ignore this email.</p>
        <p>Best regards,<br>Healify :)</p>";

            using (var message = new MailMessage("noreply@yourapp.com", email)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Ensure the email is sent as HTML
            })
            {
                using (var client = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"])))
                {
                    client.Credentials = new System.Net.NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                    client.EnableSsl = bool.Parse(_configuration["Smtp:EnableSSL"]);
                    client.UseDefaultCredentials = false;
                    await client.SendMailAsync(message);
                }
            }
        }



        public async Task<bool> ResetPassword(string email, string newPassword)
        {
            var admin = await GetAdminDetails(email);
            if (admin != null)
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                // Save the new password (e.g., update the database)
                return true;
            }

            var doctor = await GetDoctorDetails(email);
            if (doctor != null)
            {
                doctor.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                // Save the new password (e.g., update the database)
                return true;
            }

            return false;
        }

        public bool ValidateToken(string token, string email)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Check if the token has expired
                var expirationTime = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                if (expirationTime == null || long.Parse(expirationTime) < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    return false;
                }

                // Check if the token is valid for now (not before time)
                var notBeforeTime = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Nbf)?.Value;
                if (notBeforeTime != null && long.Parse(notBeforeTime) > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    return false;
                }

                // Optionally, validate other claims, such as email
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
                if (emailClaim != email)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
