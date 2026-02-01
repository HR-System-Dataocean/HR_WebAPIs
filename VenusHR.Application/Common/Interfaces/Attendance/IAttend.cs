using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Application.Common.Interfaces.Attendance
{
    public interface IAttendance
    {
        object CheckInOut(int EmployeeID,double Latitude,double longitude, DateTime CheckingDatetime, string DeviceID,string deviceModel,string osVersion,string networkType,int Lang,string CheckType);
        object GetAttendanceHistory(int EmployeeID,DateTime? FromDate=null,DateTime? ToDate =null);

    }
}
