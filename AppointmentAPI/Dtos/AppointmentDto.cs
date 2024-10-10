namespace AppointmentAPI.Dtos
{
    public class AppointmentDto
    {
        public string? AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string Speciality { get; set; }
        public string DoctorId { get; set; }
        public string SelectedSlot { get; set; }
        public string PatientStatus { get; set; }
        public string DoctorStatus { get; set; }
    }
}
