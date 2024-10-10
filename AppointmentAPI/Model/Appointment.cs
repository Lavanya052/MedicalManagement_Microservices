using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI.Model
{
    public class Appointment
    {
        public string? AppointmentId { get; set; } // Unique ID for the appointment

        [Required(ErrorMessage = "Patient name is required.")]
        public string PatientName { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Speciality is required.")]
        public string Speciality { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public string DoctorId { get; set; }

        [Required(ErrorMessage = "Selected slot is required.")]
        public string SelectedSlot { get; set; }

        [Required(ErrorMessage = "Availability ID is required.")]
        public string? AvailabilityId { get; set; } // Use nullable if it can be optional
        public string PatientStatus { get; set; }
        public string DoctorStatus { get; set; }
    }
}
