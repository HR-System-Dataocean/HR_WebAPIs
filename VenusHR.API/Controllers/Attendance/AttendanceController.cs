using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Core.Master;
using VenusHR.Infrastructure.Presistence;
using WorkFlow_EF;
using VenusHR.API.Models;
using System.ComponentModel.DataAnnotations;

namespace VenusHR.API.Controllers.Attendance
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IAttendance _attendance;
        private readonly IHRMaster _hrMaster;

        public AttendanceController(IAttendance attendance, ApplicationDBContext context, IHRMaster hrMaster)
        {
            _context = context;
            _attendance = attendance;
            _hrMaster = hrMaster;
        }

        [HttpPost, Route("api/Attendance/CheckInOut")]
        public ActionResult<ApiResponse<object>> CheckInOut([FromBody] CheckInOutRequest request, [FromQuery] int Lang, [FromQuery] string CheckType)
        {
            // Validation
            if (request == null)
            {
                var message = Lang == 1 ? "بيانات الطلب غير صحيحة" : "Invalid request data";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            if (request.EmployeeID <= 0)
            {
                var message = Lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            if (request.CheckingDatetime == null || request.CheckingDatetime == default)
            {
                var message = Lang == 1 ? "وقت التسجيل مطلوب" : "Check time is required";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var result = _attendance.CheckInOut(
                    request.EmployeeID,
                    request.Latitude ?? 0,
                    request.Longitude ?? 0,
                    request.CheckingDatetime,
                    request.DeviceID,
                    request.DeviceModel,
                    request.OSVersion,
                    request.NetworkType,
                    Lang,
                    CheckType
                );

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل تسجيل الحضور" : "Check-in/out failed";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم تسجيل الحضور بنجاح" : "Check-in/out successful";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet, Route("api/Attendance/GetHistory")]
        public ActionResult<ApiResponse<object>> GetAttendanceHistory(
           [FromQuery] int EmployeeID,
           [FromQuery] DateTime? FromDate = null,
           [FromQuery] DateTime? ToDate = null,
           [FromQuery] int Lang = 0)
        {
            // Validation
            if (EmployeeID <= 0)
            {
                var message = Lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
            {
                var message = Lang == 1 ? "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" : "From date must be before to date";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var result = _attendance.GetAttendanceHistory(EmployeeID, FromDate, ToDate);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب سجل الحضور" : "Failed to retrieve attendance history";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب سجل الحضور بنجاح" : "Attendance history retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في جلب سجل الحضور" : "Error retrieving attendance history";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet, Route("api/Attendance/DailySummary")]
        public ActionResult<ApiResponse<object>> GetDailySummary(
            [FromQuery] int EmployeeID,
            [FromQuery] DateTime? FromDate = null,
            [FromQuery] DateTime? ToDate = null,
            [FromQuery] int Lang = 0)
        {
            // Validation
            if (EmployeeID <= 0)
            {
                var message = Lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
            {
                var message = Lang == 1 ? "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" : "From date must be before to date";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var result = _attendance.GetAttendanceHistory(EmployeeID, FromDate, ToDate);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب الملخص اليومي" : "Failed to retrieve daily summary";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب الملخص اليومي بنجاح" : "Daily summary retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في جلب الملخص اليومي" : "Error retrieving daily summary";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet, Route("api/Attendance/MonthlyReport")]
        public ActionResult<ApiResponse<object>> GetMonthlyReport(
            [FromQuery] int EmployeeID,
            [FromQuery] int Year,
            [FromQuery] int Month,
            [FromQuery] int Lang = 0)
        {
            // Validation
            if (EmployeeID <= 0)
            {
                var message = Lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            if (Year < 2000 || Year > DateTime.Now.Year + 1)
            {
                var message = Lang == 1 ? "السنة غير صحيحة" : "Invalid year";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            if (Month < 1 || Month > 12)
            {
                var message = Lang == 1 ? "الشهر غير صحيح" : "Invalid month";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var startDate = new DateTime(Year, Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var result = _attendance.GetAttendanceHistory(EmployeeID, startDate, endDate);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب التقرير الشهري" : "Failed to retrieve monthly report";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب التقرير الشهري بنجاح" : "Monthly report retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في جلب التقرير الشهري" : "Error retrieving monthly report";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet, Route("api/Attendance/Employee/{employeeId}")]
        public ActionResult<ApiResponse<object>> GetEmployeeAttendance(
            int employeeId,
            [FromQuery] int Lang = 0)
        {
            // Validation
            if (employeeId <= 0)
            {
                var message = Lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var fromDate = DateTime.Now.AddDays(-30);
                var result = _attendance.GetAttendanceHistory(employeeId, fromDate, DateTime.Now);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        var message = Lang == 1 ? "فشل جلب سجل الموظف" : "Failed to retrieve employee attendance";
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم جلب سجل الموظف بنجاح" : "Employee attendance retrieved successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في جلب سجل الموظف" : "Error retrieving employee attendance";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost, Route("api/Attendance/ImportFingerprintUsers")]
        public ActionResult<ApiResponse<object>> ImportFingerprintUsers([FromBody] List<hrs_Fingerprint_Users> users, [FromQuery] int Lang = 0)
        {
            // Validation
            if (users == null || users.Count == 0)
            {
                var message = Lang == 1 ? "بيانات المستخدمين مطلوبة" : "Users data is required";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var result = _attendance.ImportFingerprintUsers(users, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? "Import failed", output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم استيراد المستخدمين بنجاح" : "Users imported successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost, Route("api/Attendance/ImportCheckInOut")]
        public ActionResult<ApiResponse<object>> ImportCheckInOut([FromBody] List<hrs_Fingerprint_CheckInOut> records, [FromQuery] int Lang = 0)
        {
            // Validation
            if (records == null || records.Count == 0)
            {
                var message = Lang == 1 ? "بيانات السجلات مطلوبة" : "Records data is required";
                return BadRequest(ApiResponse<object>.Fail(message));
            }

            try
            {
                var result = _attendance.ImportCheckInOut(records, Lang);

                if (result is GeneralOutputClass<object> output)
                {
                    if (output.ErrorCode == 0)
                    {
                        return BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? "Import failed", output.ErrorCode));
                    }

                    var successMsg = Lang == 1 ? "تم استيراد السجلات بنجاح" : "Records imported successfully";
                    return Ok(ApiResponse<object>.Ok(output.ResultObject, output.ErrorMessage ?? successMsg));
                }

                var errorMsg = Lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(errorMsg));
            }
            catch (Exception ex)
            {
                var message = Lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }
    }

    // Request Models
    public class CheckInOutRequest
    {
        [Required(ErrorMessage = "Employee ID is required")]
        public int EmployeeID { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required(ErrorMessage = "Check time is required")]
        public DateTime CheckingDatetime { get; set; }

        public string? DeviceID { get; set; }
        public string? DeviceModel { get; set; }
        public string? OSVersion { get; set; }
        public string? NetworkType { get; set; }
    }
}
