namespace PatientAPI.Model
{
    public class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string BloodGroup { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public byte[] Image { get; set; }
        public string ImageBase64 { get; set; }

        // Additional Fields
        //public string Allergies { get; set; }
        //public string MedicalConditions { get; set; }
        //public string Medications { get; set; }
        //public string EmergencyContact { get; set; }
        //public string EmergencyContactPhoneNumber { get; set; }

        //public string Nationality { get; set; }
        //public string Ethnicity { get; set; }
        //public string Language { get; set; }
        //public string InsuranceProvider { get; set; }
        //public string InsuranceNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        //public bool EmailVerified { get; set; }
        public DateTime LastLogin { get; set; }

    }

}
