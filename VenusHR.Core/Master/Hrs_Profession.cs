using System;
using System.Collections.Generic;

namespace VenusHR.Core.Master;

public partial class Hrs_Profession
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? EngName { get; set; }

    public string? ArbName { get; set; }

    public string? ArbName4S { get; set; }

    public int CompanyId { get; set; }

    public string? Remarks { get; set; }

    public int? RegUserId { get; set; }

    public int? RegComputerId { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime? CancelDate { get; set; }
}
