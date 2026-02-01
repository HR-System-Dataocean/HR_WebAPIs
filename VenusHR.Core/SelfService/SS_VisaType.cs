using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_VisaType
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? EngName { get; set; }

    public string? ArbName { get; set; }

    public DateTime? CancelDate { get; set; }
}
