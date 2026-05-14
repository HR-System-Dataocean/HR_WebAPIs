using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusHR.Core.Master
{
    [Table("hrs_Fingerprint_Users")]
    public class hrs_Fingerprint_Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        public string BADGENUMBER { get; set; }
        public string? SSN { get; set; }
        public string? NAME { get; set; }
        public string? GENDER { get; set; }
        public string? TITLE { get; set; }
        public string? PAGER { get; set; }
        public DateTime? BIRTHDAY { get; set; }
        public DateTime? HIREDDAY { get; set; }
        public string? STREET { get; set; }
        public string? CITY { get; set; }
        public string? STATE { get; set; }
        public string? ZIP { get; set; }
        public string? OPHONE { get; set; }
        public string? FPHONE { get; set; }
        public short? VERIFICATIONMETHOD { get; set; }
        public short? DEFAULTDEPTID { get; set; }
        public short? SECURITYFLAGS { get; set; }
        public short ATT { get; set; }
        public short INLATE { get; set; }
        public short OUTEARLY { get; set; }
        public short OVERTIME { get; set; }
        public short SEP { get; set; }
        public short HOLIDAY { get; set; }
        public string? MINZU { get; set; }
        public string? PASSWORD { get; set; }
        public short LUNCHDURATION { get; set; }
        public string? MVerifyPass { get; set; }
        public byte[]? PHOTO { get; set; }
        public byte[]? Notes { get; set; }
        public int? privilege { get; set; }
        public short? InheritDeptSch { get; set; }
        public short? InheritDeptSchClass { get; set; }
        public short? AutoSchPlan { get; set; }
        public int? MinAutoSchInterval { get; set; }
        public short? RegisterOT { get; set; }
        public short? InheritDeptRule { get; set; }
        public short? EMPRIVILEGE { get; set; }
        public string? CardNo { get; set; }
        public DateTime? F_PassportExpiry { get; set; }
        public DateTime? F_IDCardExpiry { get; set; }
        public DateTime? F_InsuranceExpiry { get; set; }
        public DateTime? DMedical { get; set; }
        public DateTime? DLabor { get; set; }
        public DateTime? DPassport { get; set; }
        public DateTime? Dvisa { get; set; }
        public string? Email { get; set; }
    }
}
