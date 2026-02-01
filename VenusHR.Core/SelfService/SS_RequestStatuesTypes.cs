using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VenusHR.Core.SelfService;

public partial class SS_RequestStatuesTypes
{
    [Key]
    public int Id { get; set; }

    public string? AraName { get; set; }

    public string? EngName { get; set; }
}
