using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class sys_Departments
    {
        public int ID { get; set; }
        public string?  Code { get; set; }
        public string? EngName { get; set; }
        public string? ArbName { get; set; }
        public string ?ArbName4S { get; set; }
        public int ? ParentID { get; set; }
         public int CompanyID { get; set; }
        public string? Remarks { get; set; }
        public int? RegUserID { get; set; }
        public int? RegComputerID { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CostCenterCode { get; set; }
    }
}
