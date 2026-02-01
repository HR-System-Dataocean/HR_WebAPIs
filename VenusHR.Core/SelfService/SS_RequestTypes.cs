using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_RequestTypes
{
    public int id { get; set; }
    public int? RequestId { get; set; }

    public string? RequestCode { get; set; }

    public string? RequestArbName { get; set; }

    public string? RequestEngName { get; set; }

    public bool? NotActive { get; set; }

    public int? NoOfTimes { get; set; }

    public int? TimesPeriodPerMonth { get; set; }

    public bool? AutoSerialAttach { get; set; }
    public bool? RequiredAttach { get; set; }

}
