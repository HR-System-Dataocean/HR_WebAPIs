using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class Hrs_Projects
    {
        
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("ID")]

            public int ID { get; set; }

            [Required]
            [StringLength(50)]
            public string Code { get; set; }

            [StringLength(100)]
            public string EngName { get; set; }

            [StringLength(100)]
            public string ArbName { get; set; }

            [StringLength(100)]
            public string ArbName4S { get; set; }

            [StringLength(50)]
            public string Phone { get; set; }

            [StringLength(50)]
            public string Mobile { get; set; }

            [StringLength(50)]
            public string Fax { get; set; }

            [StringLength(100)]
            public string Email { get; set; }

            [StringLength(1024)]
            public string Adress { get; set; }

            [StringLength(100)]
            public string ContactPerson { get; set; }

            public int? ProjectPeriod { get; set; }

            public int? ClaimDuration { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            [Column(TypeName = "money")]
            public decimal? CreditLimit { get; set; }

            public int? CreditPeriod { get; set; }

            public bool? IsAdvance { get; set; }

            public bool? IsHijri { get; set; }

            public int? NotifyPeriod { get; set; }

            [StringLength(8000)]
            public string CompanyConditions { get; set; }

            [StringLength(8000)]
            public string ClientConditions { get; set; }

            public bool? IsLocked { get; set; }

            public bool? IsStoped { get; set; }

            public int? BranchID { get; set; }

            [Required]
            public int CompanyID { get; set; }

            [StringLength(2048)]
            public string Remarks { get; set; }

            public int? RegUserID { get; set; }

            public int? RegComputerID { get; set; }

            [Required]
            public DateTime RegDate { get; set; }

            public DateTime? CancelDate { get; set; }

            [StringLength(8000)]
            public string WorkConditions { get; set; }

            public int? LocationID { get; set; }

            public int? AbsentTransaction { get; set; }

            public int? LeaveTransaction { get; set; }

            public int? LateTransaction { get; set; }

            public int? SickTransaction { get; set; }

            public int? OTTransaction { get; set; }

            public int? HOTTransaction { get; set; }

            [StringLength(50)]
            public string CostCenterCode1 { get; set; }

            public int? DepartmentID { get; set; }

            [StringLength(50)]
            public string CostCenterCode2 { get; set; }

            [StringLength(50)]
            public string CostCenterCode3 { get; set; }

            [StringLength(50)]
            public string CostCenterCode4 { get; set; }

            [StringLength(500)]
            public string latitude { get; set; }

            [StringLength(500)]
            public string longitude { get; set; }

            [StringLength(500)]
            public string allowedRadius { get; set; }

            public string address { get; set; }

            // Properties for calculated values
            [NotMapped]
            public double? LatitudeValue => double.TryParse(latitude, out var lat) ? lat : null;

            [NotMapped]
            public double? LongitudeValue => double.TryParse(longitude, out var lng) ? lng : null;

            [NotMapped]
            public double? AllowedRadiusValue => double.TryParse(allowedRadius, out var radius) ? radius : null;

        
    }
}