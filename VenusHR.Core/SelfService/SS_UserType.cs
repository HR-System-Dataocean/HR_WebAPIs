using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService;

public partial class SS_UserType
{
    public int Id { get; set; }

    public int? UserTypeCode { get; set; }

    public string? UserType { get; set; }
}
