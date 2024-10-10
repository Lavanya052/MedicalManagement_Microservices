using System.ComponentModel.DataAnnotations;

namespace DoctorAvailabiltyAPI.Model
{
    public class DoctorAvailability
    {
        public int AvailabilityId { get; set; }
        public string DoctorId { get; set; }  // Foreign key to the Doctor
        public DateTime Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? IntervalMinutes { get; set; }  // Appointment duration in minutes
        public bool IsAvailable { get; set; }
        public string Description { get; set; } 

    }
}
