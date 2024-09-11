using AvailabilityAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvailabilityAPI.Service
{
    public interface IAvailabilityService
    {
        Task AddDoctorAvailability(DoctorAvailability doctorAvailability);
        Task<DoctorAvailability> GetDoctorAvailability(string doctorId);
        Task UpdateDoctorAvailability(DoctorAvailability doctorAvailability);
        Task DeleteDoctorAvailability(int id);
    }
}
