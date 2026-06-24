namespace VenusHR.Core.Attendance
{
    public class ExpectedStartEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeNameAr { get; set; } = string.Empty;
        public string EmployeeNameEn { get; set; } = string.Empty;
        public string ExpectedStartTime { get; set; } = string.Empty;
        public int GraceMinutes { get; set; }
        public string AllowedStartTime { get; set; } = string.Empty;
    }
}
