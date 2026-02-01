using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class Hrs_Mobile_Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int EmployeeID { get; set; }
        public DateTime CheckingDatetime { get; set; }
        public double Latitude { get; set; }  
        public double Longitude { get; set; } 
        public string? DeviceID { get; set; }
        public string? DeviceModel { get; set; }
        public string? OSVersion { get; set; }
        public string? NetworkType { get; set; }
        public double? DistanceFromLocation { get; set; }  
        public DateTime? CreatedDate { get; set; }
        public string? CheckType { get; set; } 


    }
}
