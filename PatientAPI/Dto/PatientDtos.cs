namespace PatientAPI.Dto
{
    public class PatientInternalDto
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string BloodGroup { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }  // Sensitive data, used internally
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ImageBase64 { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
    }

    public class PatientDto
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string BloodGroup { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ImageBase64 { get; set; }

    }
}
