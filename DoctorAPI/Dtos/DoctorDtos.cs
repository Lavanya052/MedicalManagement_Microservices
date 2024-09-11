namespace DoctorAPI.Dtos
{
    public class DoctorDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class DoctorInternalDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
