using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusHR.Core.Master
{
    public class sys_MobileConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CompanyId { get; set; }
        public bool EmpCanUploadProfilePic { get; set; } = false;
        public string? Remarks { get; set; }
        public int? RegUserID { get; set; }
        public int? RegComputerID { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? CancelDate { get; set; }
    }
}
