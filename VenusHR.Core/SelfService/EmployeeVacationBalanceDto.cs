namespace VenusHR.Core.SelfService
{
    public class EmployeeVacationBalanceResultDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public int VacationTypeId { get; set; }
        public string VacationTypeName { get; set; } = string.Empty;
        public int? ContractId { get; set; }
        public DateTime BalanceDate { get; set; }

        public decimal TotalBalance { get; set; }
        public decimal EntitledBalance { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal VacationDays { get; set; }
        public bool RoundAnnualVacBalance { get; set; }

        public EmployeeVacationDetailDto? SelectedVacation { get; set; }
        public List<EmployeeVacationHistoryDto> PreviousVacations { get; set; } = new();
    }

    public class EmployeeVacationDetailDto
    {
        public int VacationId { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal EntitledBalance { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal VacationDays { get; set; }
        public bool ZeroingBalance { get; set; }
        public int? OverDueVacationId { get; set; }
        public int? OverDueVacationDays { get; set; }
        public int? VacationRequestId { get; set; }
        public DateTime? RegDate { get; set; }
    }

    public class EmployeeVacationHistoryDto
    {
        public int Id { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal TotalDays { get; set; }
        public decimal ConsumDays { get; set; }
        public decimal VacationDays { get; set; }
        public decimal OverdueDays { get; set; }
        public decimal RemainingDays { get; set; }
        public bool ZeroingBalance { get; set; }
        public int? PaymentTrnId { get; set; }
    }
}
