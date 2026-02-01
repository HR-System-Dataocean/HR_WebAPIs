using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_VFollowup
{
    public int ID { get; set; }

    public string VacationType { get; set; }
    public string RequestSerial { get; set; }
    public int EmployeeID { get; set; }
    public string EmployeeArbName { get; set; }
    public string EmployeeEngName { get; set; }
    public DateTime RequestDate { get; set; }
    public string RequestArbName { get; set; }
    public string RequestEngName { get; set; }
    public string FormCode { get; set; }
    public int RequestStautsTypeID { get; set; }
}
