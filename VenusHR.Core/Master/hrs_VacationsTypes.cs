using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class hrs_VacationsTypes
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string? EngName { get; set; }

        public string? ArbName { get; set; }

        public string? ArbName4S { get; set; }

        public short? IsPaid { get; set; }

        public string? Sex { get; set; }

        public bool? IsAnnual { get; set; }

        public bool? IsSickVacation { get; set; }

        public bool? IsFromAnnual { get; set; }

        public int? ForSalaryTransaction { get; set; }

        public int CompanyId { get; set; }

        public string? Remarks { get; set; }

        public int? RegUserId { get; set; }

        public int? RegComputerId { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public int? ObalanceTransactionId { get; set; }

        public int? OverDueVacationId { get; set; }

        public float? Stage1Pct { get; set; }

        public float? Stage2Pct { get; set; }

        public float? Stage3Pct { get; set; }

        public int? ForDeductionTransaction { get; set; }

        public bool? AffectEos { get; set; }

        public int? VactionTypeCaculation { get; set; }

        public int? ExceededDaysType { get; set; }

     }
}
