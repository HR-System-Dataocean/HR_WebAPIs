using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VenusHR.Core.SelfService;

public partial class SS_ResignationReason
{
    [Key]
    public int Code { get; set; }

    public string? ArbName { get; set; }

    public string? EngName { get; set; }

    public string? Description { get; set; }
}
