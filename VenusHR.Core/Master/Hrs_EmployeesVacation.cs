using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class Hrs_EmployeesVacations
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int VacationTypeId { get; set; }

        public DateTime? ExpectedStartDate { get; set; }

        public DateTime? ExpectedEndDate { get; set; }

        public string? EmployeeRequestRemarks { get; set; }

        public bool? IsContracial { get; set; }

        public DateTime? ActualStartDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public string? Remarks { get; set; }

        public int? RegUserId { get; set; }

        public int? RegComputerId { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public float? TotalDays { get; set; }

        public decimal? RemainingDays { get; set; }

        public float? ConsumDays { get; set; }

        public string? HijriExpectedStartDate { get; set; }

        public string? HijriExpectedEndDate { get; set; }

        public string? HijriActualStartDate { get; set; }

        public string? HijriActualEndDate { get; set; }

        public int? OverDueVacation { get; set; }

        public int? PaymentTrnId { get; set; }

        public decimal? TotalBalance { get; set; }

        public int? PaidFromBalance { get; set; }

        public decimal? RemainingBalance { get; set; }

        public int? Vactiondays { get; set; }

        public int? OverdueDays { get; set; }

        public int? ParentVacationId { get; set; }

        public bool? ZeroingBalance { get; set; }

 

    }
}
