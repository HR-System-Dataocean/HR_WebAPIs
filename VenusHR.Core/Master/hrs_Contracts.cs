using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class hrs_Contracts
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public int ContractTypeID { get; set; }
        public int EmployeeClassID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime?EndDate { get; set; }
        public int ProfessionID { get; set; }
        public int PositionID { get; set; }
        public int GradeStepID { get; set; }
        public int CurrencyID { get; set; }
        public string? Remarks { get; set; }
        public int RegUserID { get; set; }
        public int RegComputerID { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public int ContractPeriod { get; set; }

    }
}
