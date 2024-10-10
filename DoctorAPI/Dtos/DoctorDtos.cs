namespace DoctorAPI.Dtos
{
    public class DoctorDto
    {
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageBase64 { get; set; } // Add this property for Base64 image string
    }

    public class DoctorInternalDto
    {
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ImageBase64 { get; set; } // Add this property for Base64 image string
    }
}
