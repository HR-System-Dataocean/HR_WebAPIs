using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_ExperienceRate
{
    public int ID { get; set; }
    public int? Code { get; set; }

    public string? ArbName { get; set; }

    public string? EngName { get; set; }
}
