using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Json;

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
            var response = await _httpClient.GetAsync($"{apiUrl}/api/admin/internal/{username}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AdminInternalDto>();
            }

            return null;
        }

        public async Task<DoctorInternalDto> GetDoctorDetails(string username)
        {
            var apiUrl = _configuration["DoctorAPI:BaseUrl"];
            var response = await _httpClient.GetAsync($"{apiUrl}/api/doctor/internal/{username}");

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
    }
}
