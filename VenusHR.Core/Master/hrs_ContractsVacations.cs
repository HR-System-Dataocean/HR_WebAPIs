using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class hrs_ContractsVacations
    {
        public int ID { get; set; }
        public int ContractID { get; set; }
        public int VacationTypeID { get; set; }
        public double DurationDays { get; set; }
        public int RequiredWorkingMonths { get; set; }
        public int FromMonth { get; set; }
        public int? ToMonth { get; set; }
        public string? Remarks { get; set; }
        public int RegUserID { get; set; }
        public int RegComputerID { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime CancelDate { get; set; }
        public int TicketsRnd { get; set; }
        public int DependantTicketRnd { get; set; }
        public int MaxKeepDays { get; set; }
    }
}
