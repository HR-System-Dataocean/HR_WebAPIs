using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Core.Master;
using VenusHR.Infrastructure.Presistence;
using WorkFlow_EF;

namespace VenusHR.API.Controllers.Attendance
{
    public class AttendanceController : Controller
    {
        private ApplicationDBContext _context;
        private readonly IAttendance _Attendance;
        private readonly IHRMaster _HRMaster;


        public AttendanceController(IAttendance Attendance, ApplicationDBContext context)
        {
            _context = context;
            _Attendance = Attendance;
        }
        [HttpPost, Route("api/Attendance/CheckInOut")]
         public ActionResult<object> CheckIn([FromBody] Hrs_Mobile_Attendance request, [FromQuery] int Lang, [FromQuery] string CheckType)
        {
            try
            {
                 if (request == null)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "بيانات الطلب غير صحيحة" : "Invalid request data"
                    });
                }

                 var result = _Attendance.CheckInOut(
                    request.EmployeeID,
                    request.Latitude,
                    request.Longitude,
                    request.CheckingDatetime,
                    request.DeviceID,
                    request.DeviceModel,
                    request.OSVersion,
                    request.NetworkType,
                    Lang,
                    CheckType

                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ في الخادم" : "Server error occurred",
                    Error = ex.Message
                });
            }
        }
       
        
        [HttpGet, Route("api/Attendance/GetHistory")]
        public ActionResult<object> GetAttendanceHistory(
           [FromQuery] int EmployeeID,
           [FromQuery] DateTime? FromDate = null,
           [FromQuery] DateTime? ToDate = null,
           [FromQuery] int Lang = 0)
        {
            try
            {
                 if (EmployeeID <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                    });
                }

                 var result = _Attendance.GetAttendanceHistory(EmployeeID, FromDate, ToDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ في جلب سجل الحضور" : "Error retrieving attendance history",
                    Error = ex.Message
                });
            }
        }

        [HttpGet, Route("api/Attendance/DailySummary")]
        public ActionResult<object> GetDailySummary(
            [FromQuery] int EmployeeID,
            [FromQuery] DateTime? FromDate = null,
            [FromQuery] DateTime? ToDate = null,
            [FromQuery] int Lang = 0)
        {
            try
            {
                 if (EmployeeID <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                    });
                }

                 var result = _Attendance.GetAttendanceHistory(EmployeeID, FromDate, ToDate);

                 if (result is GeneralOutputClass<object> output && output.ErrorCode == 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = output.ErrorMessage
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ في جلب الملخص اليومي" : "Error retrieving daily summary",
                    Error = ex.Message
                });
            }
        }

        [HttpGet, Route("api/Attendance/MonthlyReport")]
        public ActionResult<object> GetMonthlyReport(
            [FromQuery] int EmployeeID,
            [FromQuery] int Year,
            [FromQuery] int Month,
            [FromQuery] int Lang = 0)
        {
            try
            {
                 if (EmployeeID <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                    });
                }

                if (Year < 2000 || Year > DateTime.Now.Year + 1)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "السنة غير صحيحة" : "Invalid year"
                    });
                }

                if (Month < 1 || Month > 12)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "الشهر غير صحيح" : "Invalid month"
                    });
                }

                 var startDate = new DateTime(Year, Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                 var result = _Attendance.GetAttendanceHistory(EmployeeID, startDate, endDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ في جلب التقرير الشهري" : "Error retrieving monthly report",
                    Error = ex.Message
                });
            }
        }

        [HttpGet, Route("api/Attendance/Employee/{employeeId}")]
        public ActionResult<object> GetEmployeeAttendance(
            int employeeId,
            [FromQuery] int Lang = 0)
        {
            try
            {
                 if (employeeId <= 0)
                {
                    return BadRequest(new
                    {
                        Status = false,
                        Message = (Lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                    });
                }

                 var fromDate = DateTime.Now.AddDays(-30);
                var result = _Attendance.GetAttendanceHistory(employeeId, fromDate, DateTime.Now);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = (Lang == 1) ? "حدث خطأ في جلب سجل الموظف" : "Error retrieving employee attendance",
                    Error = ex.Message
                });
            }
        }
    }
}
