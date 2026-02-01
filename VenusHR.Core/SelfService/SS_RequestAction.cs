using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_RequestAction
{
    public int ID { get; set; }
    public int ActionSerial { get; set; }

    public int RequestSerial { get; set; }

    public int Ss_EmployeeId { get; set; }

    public string? FormCode { get; set; }

    public int ConfigId { get; set; }

    public int EmployeeId { get; set; }

    public bool? Seen { get; set; }

    public int? ActionId { get; set; }

    public DateTime? ActionDate { get; set; }

    public string? ConfirmedNoOfdays { get; set; }

    public string? ActionRemarks { get; set; }

    public bool? IsHidden { get; set; }

    public double? HoursCount { get; set; }

    public DateTime? OvertimeDate { get; set; }

    public int? OvertimeType { get; set; }

    public int? MinutsCount { get; set; }
}
