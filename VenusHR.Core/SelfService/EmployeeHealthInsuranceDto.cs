namespace VenusHR.Core.SelfService
{
    public class EmployeeHealthInsuranceResultDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int? ContractId { get; set; }
        public HealthInsuranceDto HealthInsurance { get; set; } = new();
        public TravelTicketDto TravelTicket { get; set; } = new();
        public float? WorkHours { get; set; }
        public bool? IsProjectRelated { get; set; }
        public bool? IsSpecialForce { get; set; }
    }

    public class HealthInsuranceDto
    {
        public string InsuranceCompany { get; set; } = string.Empty;
        public string InsuranceClass { get; set; } = string.Empty;
        public decimal? EmployeeAmount { get; set; }
        public decimal? CompanyAmount { get; set; }
        public DateTime? ActivationDate { get; set; }
    }

    public class TravelTicketDto
    {
        public int? TicketClassId { get; set; }
        public string TicketClassName { get; set; } = string.Empty;
        public int? TicketRouteId { get; set; }
        public string TicketRouteName { get; set; } = string.Empty;
        public decimal? TotalCost { get; set; }
        public bool? IsPaid { get; set; }
    }
}
