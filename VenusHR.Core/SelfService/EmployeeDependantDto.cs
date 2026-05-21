namespace VenusHR.Core.SelfService
{
    public class EmployeeDependantsResultDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int ObjectId { get; set; }
        public List<EmployeeDependantDto> Dependants { get; set; } = new();
    }

    public class EmployeeDependantDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? DependantTypeId { get; set; }
        public string EnglishName { get; set; } = string.Empty;
        public string ArabicName { get; set; } = string.Empty;
        public string? ArabicName4S { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? BirthCityId { get; set; }
        public string? Sex { get; set; }
        public int? NationalityId { get; set; }
        public bool? InsuranceCovered { get; set; }
        public decimal? InsurancePercentage { get; set; }
        public bool? TicketCovered { get; set; }
        public decimal? TicketPercentage { get; set; }
        public string? NationalIdOrIqamaNo { get; set; }
        public string? Remarks { get; set; }
        public DateTime? RegDate { get; set; }
        public string? FileName { get; set; }
        public int ObjectId { get; set; }
    }
}
