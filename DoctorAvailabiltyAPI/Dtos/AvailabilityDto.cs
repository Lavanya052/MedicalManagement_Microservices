namespace DoctorAvailabiltyAPI.Dtos
{
    public class AvailabilityDto
    {
        public string DoctorId { get; set; }
        public DateTime Date { get; set; }
        public String StartTime { get; set; }
        public String EndTime { get; set; }
        public int? IntervalMinutes { get; set; }
        public bool IsAvailable { get; set; }

        public string Description { get; set; }
    }

    public class AvailabilityInternalDto
    {
        public int AvailabilityId { get; set; }
        public string DoctorId { get; set; }
        public DateTime Date { get; set; }
        public String StartTime { get; set; }
        public String EndTime { get; set; }
        public int? IntervalMinutes { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
    }
}
