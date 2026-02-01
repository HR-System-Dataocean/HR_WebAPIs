using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{
    
   
//        [Table("hrs_EmployeesClasses")]
        public class hrs_EmployeesClasses
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int ID { get; set; }

            [Required]
            [StringLength(50)]
            public string Code { get; set; }

            [StringLength(100)]
            public string EngName { get; set; }

            [StringLength(100)]
            public string ArbName { get; set; }

            [StringLength(100)]
            public string ArbName4S { get; set; }

            public byte? NoOfDaysPerPeriod { get; set; }

            public float? WorkHoursPerDay { get; set; }

            public short? NoOfHoursPerWeek { get; set; }

            public short? NoOfHoursPerPeriod { get; set; }

            public float? OvertimeFactor { get; set; }

            public float? HolidayFactor { get; set; }

            public byte? FirstDayOfWeek { get; set; }

            public DateTime? DefultStartTime { get; set; }

            public DateTime? DefultEndTime { get; set; }

            public bool? WorkingUnitsIsHours { get; set; }

            public int? DefaultProjectID { get; set; }

            [Required]
            public int CompanyID { get; set; }

            [StringLength(2048)]
            public string Remarks { get; set; }

            [Required]
            public int RegUserID { get; set; }

            public int? RegComputerID { get; set; }

            [Required]
            public DateTime RegDate { get; set; }

            public DateTime? CancelDate { get; set; }

            public int? NonPermiLatTransaction { get; set; }

            public int? PerDailyDelaying { get; set; }

            public int? PerMonthlyDelaying { get; set; }

            public int? NonProfitOverTimeH { get; set; }

            [StringLength(2048)]
            public string EOBFormula { get; set; }

            [StringLength(2048)]
            public string OvertimeFormula { get; set; }

            [StringLength(2048)]
            public string HolidayFormula { get; set; }

            public int? OvertimeTransaction { get; set; }

            public int? HOvertimeTransaction { get; set; }

            public bool? PolicyCheckMachine { get; set; }

            public bool? HasAttendance { get; set; }

            public int? PunishementCalc { get; set; }

            public int? OnNoExit { get; set; }

            public int? DeductionMethod { get; set; }

            public int? MaxLoanAmtPCT { get; set; }

            public int? MinServiceMonth { get; set; }

            public int? MaxInstallementPCT { get; set; }

            public int? EOSCostingTrns { get; set; }

            public int? TicketsCostingTrns { get; set; }

            public int? VacCostingTrns { get; set; }

            public int? HICostingTrns { get; set; }

            public int? TravalTrans { get; set; }

            [StringLength(2048)]
            public string AbsentFormula { get; set; }

            [StringLength(2048)]
            public string LateFormula { get; set; }

            [StringLength(2048)]
            public string VacCostFormula { get; set; }

            public bool? HasFingerPrint { get; set; }

            public bool? HasOvertimeList { get; set; }

            public bool? AttendanceFromTimeSheet { get; set; }

            public bool? HasFlexibleTime { get; set; }

            public bool? HasFlexableFingerPrint { get; set; }

            public bool? AdvanceBalance { get; set; }

            public bool? VacationTrans { get; set; }

            public int? VactionTransType { get; set; }

            public int? TransValue { get; set; }

            public bool? AddBalanceInAddEmp { get; set; }

            public bool? AccumulatedBalance { get; set; }
        }
    
 
 
}
