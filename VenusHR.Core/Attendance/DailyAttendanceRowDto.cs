namespace VenusHR.Core.Attendance
{
    public class DailyAttendanceRowDto
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int? DepartmentID { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Department { get; set; }
        public string? FirstCheckIn { get; set; }
        public string? LastCheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
    }
}
