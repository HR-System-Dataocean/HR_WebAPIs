using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_EndOfServiceRequest
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int EmployeeId { get; set; }

    public DateTime RequestDate { get; set; }

    public int? EostypeId { get; set; }

    public DateTime Eosdate { get; set; }

    public string? Eosreasons { get; set; }

    public string? Eosremarks { get; set; }

    public int? ServiceYears { get; set; }

    public int? SerciveMonths { get; set; }

    public int? ServiceDays { get; set; }

    public string? FormCode { get; set; }

    public string? RequesterUser { get; set; }

    public int? ResignationReasonCode { get; set; }

    public int? DepartmentRateCode { get; set; }

    public string? OtherResignationReason { get; set; }

    public int? UsRateCode { get; set; }

    public string? CanBack { get; set; }

    public int? RequestStautsTypeId { get; set; }
}
