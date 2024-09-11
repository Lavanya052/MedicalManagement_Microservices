using System.Threading.Tasks;
using AuthAPI.Models;

namespace AuthAPI.Services
{
    public interface IAuthService
    {
        Task<AdminInternalDto> GetAdminDetails(string username);
        Task<DoctorInternalDto> GetDoctorDetails(string username);
        string GenerateJwtToken(string username);
    }
}
