using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusHR.Core.SelfService;

public partial class SS_ContactInformationUpdate
{
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("Code")]
    public string Code { get; set; }

    [Required]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column("RequestDate")]
    public DateTime RequestDate { get; set; }

    [StringLength(5000)]
    [Column("Remarks")]
    public string? Remarks { get; set; }

    [StringLength(50)]
    [Column("RequesterUser")]
    public string? RequesterUser { get; set; }
    public int? RequestStautsTypeId { get; set; }

}