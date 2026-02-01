using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    public class sys_Nationalities
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string EngName { get; set; }
        public string ArbName { get; set; }
        public string ArbName4S { get; set; }
        public bool IsMainNationality { get; set; }
        public int TravelRoute { get;}
        public int TravelClass { get;}
        public string Remarks { get; set;}
        public int RegUserID { get; set;}
        public int RegComputerID { get; set;}
        public DateTime RegDate { get; set;}
        public DateTime CancelDate { get; set;}
        public decimal TicketAmount { get; set;}
        public decimal TicketPrice { get; set;}
                    


    }
}
