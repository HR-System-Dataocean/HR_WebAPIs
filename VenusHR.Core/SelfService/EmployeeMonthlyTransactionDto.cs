namespace VenusHR.Core.SelfService
{
    public class EmployeeMonthlyTransactionDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int FiscalPeriodId { get; set; }
        public string FinancialPeriod { get; set; } = string.Empty;
        public bool IsPrepared { get; set; }
        public bool HideNotPaid { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal DelayHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal HolidayWorkHours { get; set; }
        public decimal NumberOfWorkDays { get; set; }
        public decimal DailySalary { get; set; }
        public decimal HourlySalary { get; set; }
        public decimal OvertimeValueHourly { get; set; }
        public decimal HolidayWorkValue { get; set; }

        public List<MonthlyTransactionLineItemDto> Entitlements { get; set; } = new();
        public List<MonthlyTransactionLineItemDto> Deductions { get; set; } = new();

        public decimal TotalEntitlements { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }
    }

    public class MonthlyTransactionLineItemDto
    {
        public int TransactionTypeId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string PaidStatus { get; set; } = string.Empty;
        public int? EmpSchId { get; set; }
    }
}
