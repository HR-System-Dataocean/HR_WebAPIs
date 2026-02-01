using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.SelfService
{
    public class SS_Configuration
    {
        public int ID { get; set; }
        public string? FormCode { get; set; }
        public string? UserTypeID { get; set; }
        public string? PositionID { get; set; }
        public string? EmployeeID { get; set; }
        public bool CanEdit { get; set; }
        public int Rank { get; set; }
        public bool IsFinal { get; set; }
        public bool ApplyForAll { get; set; }
        public bool CanBeCanceledInThisLevel { get; set; }

    }
}
