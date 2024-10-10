using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI.Model
{
    public class BookedAppointment
    {

        [Key]
        public string BookedId { get; set; }
        public string AvailabilityId { get; set; } // ID for the availability
        public string Time { get; set; } // The time of the appointment
        public bool StatusBooked { get; set; } // Status to indicate if booked

        public string AppointmentId { get; set; }
    }

}
