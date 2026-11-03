namespace VenusHR.Core.Attendance
{
    public class PeriodAttendanceEmployeeDto
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int? DepartmentID { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Department { get; set; }
        public int WorkDays { get; set; }
        public int CompletedDays { get; set; }
        public int MissingCheckoutDays { get; set; }
        public int NoMovementDays { get; set; }
        public string TotalDuration { get; set; } = "0:00";
        public long TotalDurationMinutes { get; set; }
    }
}
