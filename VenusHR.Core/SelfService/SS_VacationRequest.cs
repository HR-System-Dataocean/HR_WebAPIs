using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.SelfService
{
    public class SS_VacationRequest
    {

        public int ID { get; set; }
        public string? Code { get; set; }
        
        public int VacationTypeID { get; set; }
        public string? VacationType { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalBalance { get; set; }
        public int NoOfDays { get; set; }
        public string? ContactNo { get; set; }
        public int AlternativeUser { get; set; }
        public string? Remarks { get; set; }
        public string? RegUser { get; set; }
        public DateTime? RegDate { get; set; }
        public int EmployeeID { get; set; }
        public int? RequestStautsTypeId { get; set; }


    }
}
