
using System.Collections.Generic;
namespace AvailabilityAPI.Model

{
    public class Slot
    {
        public int Id { get; set; } // Primary key
        public string StartTime { get; set; } // Store in "HH:mm" format
        public string EndTime { get; set; }   // Store in "HH:mm" format
        public int Interval { get; set; }     // Interval in minutes
        public bool IsAvailable { get; set; } // Availability status
        public int AvailabilityId { get; set; } // Foreign key
        public Availability Availability { get; set; } // Navigation property
    }

    public class Availability
    {
        public int Id { get; set; } // Primary key
        public string Date { get; set; } // Store date in "yyyy-MM-dd" format
        public List<Slot> Slots { get; set; } // Navigation property
        public int DoctorAvailabilityId { get; set; } // Foreign key
        public DoctorAvailability DoctorAvailability { get; set; } // Navigation property
    }

    public class DoctorAvailability
    {
        public int Id { get; set; } // Primary key
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public List<Availability> AvailabilityData { get; set; } // Navigation property
    }
}
