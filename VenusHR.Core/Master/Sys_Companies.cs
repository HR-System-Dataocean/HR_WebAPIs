using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace VenusHR.Core.Master
{
    
        [Table("sys_Companies")]
        public class Sys_Companies
    {
              public int ID { get; set; }

              public string Code { get; set; }

             public string? EngName { get; set; }

             public string? ArbName { get; set; }

             public string? ArbName4S { get; set; }

            public bool? IsHigry { get; set; }

            public bool? IncludeAbsencDays { get; set; }

             public string? EmpFirstName { get; set; }

             public string? EmpSecondName { get; set; }

             public string? EmpThirdName { get; set; }

             public string? EmpFourthName { get; set; }

             public string? EmpNameSeparator { get; set; }

             public string? Remarks { get; set; }

            public int? RegUserID { get; set; }

            public int? RegComputerID { get; set; }

             public DateTime RegDate { get; set; }

            public DateTime? CancelDate { get; set; }

            public int? PrepareDay { get; set; }

             public string? DefaultTheme { get; set; }

            public bool? VacationIsAccum { get; set; }

            public bool? HasSequence { get; set; }

            public int? SequenceLength { get; set; }

            public int? Prefix { get; set; }

             public string? Separator { get; set; }

            public byte? SalaryCalculation { get; set; }

            public bool? DefaultAttend { get; set; }

            public bool? CountEmployeeVacationDaysTotal { get; set; }

            public bool? ZeroBalAfterVac { get; set; }

            public bool? VacSettlement { get; set; }

            public bool? AllowOverVacation { get; set; }

            public bool? VacationFromPrepareDay { get; set; }

            public int? ExecuseRequestHoursallowed { get; set; }

            public bool? EmployeeDocumentsAutoSerial { get; set; }

            public bool? UserDepartmentsPermissions { get; set; }
        }
    }
