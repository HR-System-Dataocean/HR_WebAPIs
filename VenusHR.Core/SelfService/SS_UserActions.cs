using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_UserActions
{
    public int Id { get; set; }

    public string? ActionCode { get; set; }

    public string? ActionAraName { get; set; }

    public string? ActionEngName { get; set; }
}
