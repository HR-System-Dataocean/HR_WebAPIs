using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class hrs_Educations
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string? EngName   { get; set; }
        public string? ArbName { get; set;}
        public string? ArbName4S { get; set;}
        public int Level { get; set;}
        public int RequiredYears { get; set;}
        public int CompanyID { get; set;}
        public string? Remarks { get; set;}
        public int RegUserID { get; set;}
        public int RegComputerID { get; set;}
        public DateTime RegDate { get; set;}
        public DateTime CancelDate { get; set;}




    }
}
