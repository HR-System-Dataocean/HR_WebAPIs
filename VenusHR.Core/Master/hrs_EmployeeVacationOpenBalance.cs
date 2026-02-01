using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class hrs_EmployeeVacationOpenBalance
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public int VacationTypeID { get; set; }
        public DateTime GBalanceDate { get; set; }
        public DateTime HBalanceDate { get; set; }
        public decimal Days { get; set; }
        public int OverDue { get; set; }
        public decimal VacationBalance { get; set; }
        public int RegUserID { get; set; }
        public int RegComputerID { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime? CancelDate { get; set; }

    }
}
