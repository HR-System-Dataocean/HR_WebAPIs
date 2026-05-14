using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusHR.Core.Master
{
    [Table("hrs_Fingerprint_CheckInOut")]
    public class hrs_Fingerprint_CheckInOut
    {
        public int USERID { get; set; }
        public DateTime? Auto_Date { get; set; }
        public DateTime CHECKTIME { get; set; }
        public string? CHECKTYPE { get; set; }
        public int? VERIFYCODE { get; set; }
        public string? SENSORID { get; set; }
        public string? Memoinfo { get; set; }
        public int? WorkCode { get; set; }
        public string? sn { get; set; }
        public short? UserExtFmt { get; set; }
        public string? Remarks { get; set; }
        public int? IsManual { get; set; }
    }
}
