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

    [StringLength(255)]
    [Column("E_Mail")]
    public string? EMail { get; set; }

    [StringLength(100)]
    [Column("Phone")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [Column("Mobile")]
    public string? Mobile { get; set; }

    [StringLength(500)]
    [Column("AddressAsPerContract")]
    public string? AddressAsPerContract { get; set; }

    public int? RequestStautsTypeId { get; set; }

}