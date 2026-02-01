using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class Hrs_Employees
    {
        public int id { get; set; }
        public string Code { get; set; } = null!;

        public string? OldCode { get; set; }

        public string? EngName { get; set; }

        public string? ArbName { get; set; }

        public string? ArbName4S { get; set; }

        public string? FamilyEngName { get; set; }

        public string? FamilyArbName { get; set; }

        public string? FamilyArbName4S { get; set; }

        public string? FatherEngName { get; set; }

        public string? FatherArbName { get; set; }

        public string? FatherArbName4S { get; set; }

        public string? GrandEngName { get; set; }

        public string? GrandArbName { get; set; }

        public string? GrandArbName4S { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? BirthCityId { get; set; }

        public int? ReligionId { get; set; }

        public int? MaritalStatusId { get; set; }

        public string? Sex { get; set; }

        public int? BloodGroupId { get; set; }

        public int? BankId { get; set; }

        public int? NationalityId { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? BankAccNumber { get; set; }

        public int? DepartmentId { get; set; }

        public string? Gosinumber { get; set; }

        public DateTime? GosijoinDate { get; set; }

        public DateTime? GosiexcludeDate { get; set; }

        public DateTime? JoinDate { get; set; }

        public DateTime? ExcludeDate { get; set; }

        public int CompanyId { get; set; }

        public string? Remarks { get; set; }

        public int RegUserId { get; set; }

        public int? RegComputerId { get; set; }

        public DateTime RegDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public int? BranchId { get; set; }

        public int? SponsorId { get; set; }

        public string? E_Mail { get; set; }

        public string? Phone { get; set; }

        public string? Mobile { get; set; }

        public int? ManagerId { get; set; }

        public string? MachineCode { get; set; }

        public int? SectorId { get; set; }

        public string? SsnNo { get; set; }

        public string? PassPortNo { get; set; }

        public string? EntryNo { get; set; }

        public int? Cost1 { get; set; }

        public int? Cost2 { get; set; }

        public int? Cost3 { get; set; }

        public int? Cost4 { get; set; }

        public string? LaborOfficeNo { get; set; }

        public int? LocationId { get; set; }

        public float? Whours { get; set; }

        public bool? IsProjectRelated { get; set; }

        public bool? IsSpecialForce { get; set; }

        public double? MaxLoanDedution { get; set; }

        public string? LedgerCode { get; set; }
        public string? WorkE_Mail { get; set; }

        public bool? HasTaqat { get; set; }

        public string? BankAccountType { get; set; }

        //public virtual ICollection<HrsContract> HrsContracts { get; } = new List<HrsContract>();

     }
}
