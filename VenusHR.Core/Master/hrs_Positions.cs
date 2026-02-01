using System;
using System.Collections.Generic;

namespace VenusHR.Core.Master;

public partial class hrs_Positions
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? EngName { get; set; }

    public string? ArbName { get; set; }

    public string? ArbName4S { get; set; }

    public int CompanyId { get; set; }
    public int? ParentID { get; set; }
    public int? PositionLevelID { get; set; }
    public int? EvalEvaluationID { get; set; }
    public int? EvalRecruitmentID { get; set; }
    public int? EmployeesNo { get; set; }
    public int? AppraisalTypeGroupID { get; set; }
    public bool ApplyValidation { get; set; }

    public string? PositionBudget { get; set; }
    public string? Remarks { get; set; }

    public int? RegUserId { get; set; }

    public int? RegComputerId { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime? CancelDate { get; set; }
}
