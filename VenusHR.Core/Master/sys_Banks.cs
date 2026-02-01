using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace VenusHR.Core.Master
{
    public class sys_Locations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public int? CityID { get; set; }

        public int? BranchID { get; set; }

        public int? StoreID { get; set; }

        public int? InventoryCostLedgerID { get; set; }

        public int? InventoryAdjustmentLedgerID { get; set; }

        public int? CompanyID { get; set; }

        [StringLength(2048)]
        public string Remarks { get; set; }

        public int? RegUserID { get; set; }

        public int? RegComputerID { get; set; }

        [Required]
        public DateTime RegDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public int? DepartmentID { get; set; }

        [StringLength(50)]
        public string CostCenterCode1 { get; set; }

        [StringLength(50)]
        public string CostCenterCode2 { get; set; }

        [StringLength(50)]
        public string CostCenterCode3 { get; set; }

        [StringLength(50)]
        public string CostCenterCode4 { get; set; }

         public double? latitude { get; set; }

         public double? longitude { get; set; }

        public int? allowedRadius { get; set; }

        public string? address { get; set; }
    }
}
