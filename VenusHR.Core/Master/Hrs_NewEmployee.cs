using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VenusHR.Core.Master;

public  class Hrs_NewEmployee
{
    [Key]
    public int Id { get; set; }
    public string Ssno { get; set; } = null!;
    [Required]
    public DateTime SSNOIssueDate { get; set;}
    [Required]
    public DateTime SSNOExpireDate { get; set; }
    [Required]
    public string? FirstNameEnglish { get; set; }
    [Required]
    public string? FatherNameEnglish { get; set; }
    [Required]

    public string? GrandfatherNameEnglish { get; set; }
    [Required]
    public string? FamilyNameEnglish { get; set; }
    [Required]
    public string? FirstNameArabic { get; set; }
    [Required]
    public string? FatherNameArabic { get; set; }
    [Required]
    public string? GrandfatherNameArabic { get; set; }
    [Required]
    public string? FamilyNameArabic { get; set; }
    [Required]
    public DateTime? BirthDate { get; set; }
    [Required]
    public int BirthCityId { get; set; }
    [Required]
    public int NationalityId { get; set; }
    [Required]
    public int ReligionId { get; set; }
    [Required]
    public int GenderId { get; set; }
    [Required]
    public int MaritalStatusId { get; set; }
    [Required]
    public int BloodGroupId { get; set; }
    [Required]
    public string PersonalEmail { get; set; } = string.Empty;
    [Required]
    public string Mobile { get; set; } = string.Empty;
    [Required]
    public string PassportNo { get; set; } = string.Empty;
    [Required]
    public DateTime PassportIssueDate { get; set; }
    [Required]
    public DateTime PassportExpireDate { get; set; }
    [Required]
    public int BankId { get; set; }
    [Required]
    [MaxLength(14)]
    public string Ibanno { get; set; } = string.Empty;
    [Required]
    public int LastEducationCertificateId { get; set; }
    [Required]
    public DateTime GraduationDate { get; set; }
    [Required]
    public string Remarks { get; set; } = string.Empty;
    [Required]
    public bool IsTransfered { get; set; }
    [Required]
    public int ProfissionID { get; set; }

    [Required]
    [StringLength(500)]
    public string AddressAsPerContract { get; set; } = string.Empty;
}
