using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Core.Commom;

namespace VenusHR.Core.Master
{
    public class sys_Cities : BaseEntity<int>
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string EngName { get; set; }
        public string ArbName { get; set;}
        public string? ArbName4S { get; set; }
        public string? PhoneKey { get; set; }
        public int RegionID { get; set; }
        public string? TimeZone { get; set; }
        public int CountryID { get; set; }
        public string? Remarks { get; set; }
        public int RegUserID { get; set; }
        public int RegComputerID { get; set; }
        public DateTime RegDate { get; set; }
 
    }
}
