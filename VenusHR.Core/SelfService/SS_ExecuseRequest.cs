using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_ExecuseRequest
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int EmployeeId { get; set; }

    public DateTime RequestDate { get; set; }

    public string? ExecuseType { get; set; }

    public string? ExecuseReason { get; set; }

    public DateTime ExecuseDate { get; set; }

    public string? ExecuseTime { get; set; }

    public string? ExecuseShift { get; set; }

    public string? ExecuseRemarks { get; set; }

    public string? RequesterUser { get; set; }

    public int? RequestStautsTypeId { get; set; }
}
