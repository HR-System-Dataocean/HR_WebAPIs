using System;
using System.Collections.Generic;

namespace VenusHR.Core.Login;

public  class SysUser
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? EngName { get; set; }

    public string? ArbName { get; set; }

    public string? ArbName4S { get; set; }

    public string? Password { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsArabic { get; set; }

    public bool? CanChangePassword { get; set; }

    public bool? ResetSearchCriteria { get; set; }

    public bool? ResetReportCriteria { get; set; }

    public byte? SessionIdleTime { get; set; }

    public bool? EnforceAlphaNumericPwd { get; set; }

    public DateTime? PasswordExpiry { get; set; }

    public DateTime? PasswordChangedOn { get; set; }

    public string? Remarks { get; set; }

    public int? RegUserId { get; set; }

    public int? RegComputerId { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime? CancelDate { get; set; }


    public int? RelEmployee { get; set; }

    public int? LevelId { get; set; }
    public string? DeviceToken { get; set; }
}
