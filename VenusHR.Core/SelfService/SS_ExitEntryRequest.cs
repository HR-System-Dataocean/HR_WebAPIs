using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VenusHR.Core.SelfService;

public partial class SS_ExitEntryRequest
{
     public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int EmployeeId { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Remarks { get; set; }

    public DateTime ExitDate { get; set; }

    public DateTime EntryDate { get; set; }

    public string? RequesterUser { get; set; }

    public int? RequestStautsTypeId { get; set; }
}
