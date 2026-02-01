using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_OvertimeRequest
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int EmployeeId { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Remarks { get; set; }

    public string? RequesterUser { get; set; }

    public int? RequestStautsTypeId { get; set; }

    public double? HoursCount { get; set; }

    public DateTime OvertimeDate { get; set; }

    public int? OvertimeType { get; set; }

    public int? MinutsCount { get; set; }
}
