using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class hrs_LocationGeoPoints
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int LocationID { get; set; }

        public int EmployeeID { get; set; }

        public int LineNum { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(100)]
        public string? EngName { get; set; }

        [StringLength(100)]
        public string? ArbName { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? AllowedRadius { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public bool? Active { get; set; }

        public int? RegUserID { get; set; }

        public DateTime? RegDate { get; set; }

        public DateTime? CancelDate { get; set; }
    }
}
