using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Core.Attendance;
using VenusHR.Core.Master;

namespace VenusHR.Application.Common.Interfaces.Attendance
{
    public interface IAttendance
    {
        object CheckInOut(int EmployeeID,double Latitude,double longitude, DateTime CheckingDatetime, string DeviceID,string deviceModel,string osVersion,string networkType,int Lang,string CheckType, string? macAddress = null);
        object GetAttendanceHistory(int EmployeeID,DateTime? FromDate=null,DateTime? ToDate =null);
        object GetManagerTeamAttendance(int ManagerID, DateTime? Date, int Lang);
        object GetManagerTeamAttendanceByTime(int ManagerID, DateTime? Date, string? Time, int Lang);
        object GetAllEmployeesAttendance(int RequesterID, DateTime? Date, int? DepartmentID, int Lang);
        object GetAllEmployeesAttendanceByTime(int RequesterID, DateTime? Date, string? Time, int? DepartmentID, int Lang);
        object GetAllEmployeesPeriodAttendance(int RequesterID, DateTime FromDate, DateTime ToDate, int? DepartmentID, int Lang);
        object GetEmployeePeriodAttendanceDetail(int EmployeeID, DateTime FromDate, DateTime ToDate, int Lang);

        object GetRegisteredDevice(int EmployeeID, int Lang);
        object ChangeDevice(int EmployeeID, string DeviceID, int Lang);
        object ClearDevice(int EmployeeID, int Lang);
        object GetEmployeesExpectedStartBeforeTime(int? hour, int? minute, int Lang);
        List<ExpectedStartEmployeeDto> GetExpectedStartEmployeesBeforeTime(int? hour, int? minute);

        object ImportFingerprintUsers(List<hrs_Fingerprint_Users> users, int Lang);
        object ImportCheckInOut(List<hrs_Fingerprint_CheckInOut> records, int Lang);

    }
}
