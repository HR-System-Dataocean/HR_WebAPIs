using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Core.Master;
using WorkFlow_EF;
using VenusHR.API.Models;
using System.ComponentModel.DataAnnotations;

public static class AttendanceEndpoints
{
    public static void MapAttendanceEndpoints(this WebApplication app)
    {
        // Check In
        app.MapPost("/api/attendance/check-in", CheckIn).RequireAuthorization();

        // Check Out
        app.MapPost("/api/attendance/check-out", CheckOut).RequireAuthorization();

        // Get Attendance History
        app.MapGet("/api/attendance/history", GetAttendanceHistory).RequireAuthorization();

        // Get Profile
        app.MapGet("/api/attendance/profile", GetProfile).RequireAuthorization();
    }

    // =========== Check In ===========

    private static async Task<IResult> CheckIn(
        [FromBody] CheckInOutRequest request,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        // Validation
        if (request == null)
        {
            var message = lang == 1 ? "بيانات الطلب غير صحيحة" : "Invalid request data";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (request.EmployeeID <= 0)
        {
            var message = lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (request.CheckingDatetime == null || request.CheckingDatetime == default)
        {
            var message = lang == 1 ? "وقت التسجيل مطلوب" : "Check-in time is required";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        try
        {
            var result = attendanceService.CheckInOut(
                request.EmployeeID,
                request.Latitude ?? 0,
                request.Longitude ?? 0,
                request.CheckingDatetime,
                request.DeviceID,
                request.DeviceModel,
                request.OSVersion,
                request.NetworkType,
                lang,
                "IN"
            );

            if (result is GeneralOutputClass<object> output)
            {
                if (output.ErrorCode == 0)
                {
                    return Results.Json(ApiResponse<object>.Fail(
                        output.ErrorMessage ?? (lang == 1 ? "فشل تسجيل الحضور" : "Check-in failed"),
                        output.ErrorCode), statusCode: 400);
                }

                return Results.Ok(ApiResponse<object>.Ok(
                    output.ResultObject,
                    output.ErrorMessage ?? (lang == 1 ? "تم تسجيل الحضور بنجاح" : "Check-in successful")));
            }

            var errorMsg = lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
            return Results.Json(ApiResponse<object>.Fail(errorMsg), statusCode: 500);
        }
        catch (Exception ex)
        {
            var message = lang == 1 ? "حدث خطأ في تسجيل الحضور" : "Error during check-in";
            return Results.Json(ApiResponse<object>.Fail(message, 500, ex.Message), statusCode: 500);
        }
    }

    // =========== Check Out ===========

    private static async Task<IResult> CheckOut(
        [FromBody] CheckInOutRequest request,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        // Validation
        if (request == null)
        {
            var message = lang == 1 ? "بيانات الطلب غير صحيحة" : "Invalid request data";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (request.EmployeeID <= 0)
        {
            var message = lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (request.CheckingDatetime == null || request.CheckingDatetime == default)
        {
            var message = lang == 1 ? "وقت التسجيل مطلوب" : "Check-out time is required";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        try
        {
            var result = attendanceService.CheckInOut(
                request.EmployeeID,
                request.Latitude ?? 0,
                request.Longitude ?? 0,
                request.CheckingDatetime,
                request.DeviceID,
                request.DeviceModel,
                request.OSVersion,
                request.NetworkType,
                lang,
                "OUT"
            );

            if (result is GeneralOutputClass<object> output)
            {
                if (output.ErrorCode == 0)
                {
                    return Results.Json(ApiResponse<object>.Fail(
                        output.ErrorMessage ?? (lang == 1 ? "فشل تسجيل الانصراف" : "Check-out failed"),
                        output.ErrorCode), statusCode: 400);
                }

                return Results.Ok(ApiResponse<object>.Ok(
                    output.ResultObject,
                    output.ErrorMessage ?? (lang == 1 ? "تم تسجيل الانصراف بنجاح" : "Check-out successful")));
            }

            var errorMsg = lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
            return Results.Json(ApiResponse<object>.Fail(errorMsg), statusCode: 500);
        }
        catch (Exception ex)
        {
            var message = lang == 1 ? "حدث خطأ في تسجيل الانصراف" : "Error during check-out";
            return Results.Json(ApiResponse<object>.Fail(message, 500, ex.Message), statusCode: 500);
        }
    }

    // =========== Get Attendance History ===========

    private static async Task<IResult> GetAttendanceHistory(
        [FromQuery] int employeeID,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        // Validation
        if (employeeID <= 0)
        {
            var message = lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
        {
            var message = lang == 1 ? "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" : "From date must be before to date";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        try
        {
            var result = attendanceService.GetAttendanceHistory(employeeID, fromDate, toDate);

            if (result is GeneralOutputClass<object> output)
            {
                if (output.ErrorCode == 0)
                {
                    return Results.Json(ApiResponse<object>.Fail(
                        output.ErrorMessage ?? (lang == 1 ? "فشل جلب سجل الحضور" : "Failed to retrieve attendance history"),
                        output.ErrorCode), statusCode: 400);
                }

                return Results.Ok(ApiResponse<object>.Ok(
                    output.ResultObject,
                    output.ErrorMessage ?? (lang == 1 ? "تم جلب سجل الحضور بنجاح" : "Attendance history retrieved successfully")));
            }

            var errorMsg = lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
            return Results.Json(ApiResponse<object>.Fail(errorMsg), statusCode: 500);
        }
        catch (Exception ex)
        {
            var message = lang == 1 ? "حدث خطأ في جلب سجل الحضور" : "Error retrieving attendance history";
            return Results.Json(ApiResponse<object>.Fail(message, 500, ex.Message), statusCode: 500);
        }
    }

    // =========== Get Profile ===========

    private static async Task<IResult> GetProfile(
        [FromQuery] int employeeID,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        // Validation
        if (employeeID <= 0)
        {
            var message = lang == 1 ? "معرف الموظف غير صحيح" : "Invalid employee ID";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        try
        {
            var fromDate = DateTime.Now.AddDays(-30);
            var result = attendanceService.GetAttendanceHistory(employeeID, fromDate, DateTime.Now);

            if (result is GeneralOutputClass<object> output)
            {
                if (output.ErrorCode == 0)
                {
                    return Results.Json(ApiResponse<object>.Fail(
                        output.ErrorMessage ?? (lang == 1 ? "فشل جلب الملف الشخصي" : "Failed to retrieve profile"),
                        output.ErrorCode), statusCode: 400);
                }

                return Results.Ok(ApiResponse<object>.Ok(
                    output.ResultObject,
                    output.ErrorMessage ?? (lang == 1 ? "تم جلب الملف الشخصي بنجاح" : "Profile retrieved successfully")));
            }

            var errorMsg = lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
            return Results.Json(ApiResponse<object>.Fail(errorMsg), statusCode: 500);
        }
        catch (Exception ex)
        {
            var message = lang == 1 ? "حدث خطأ في جلب الملف الشخصي" : "Error retrieving profile";
            return Results.Json(ApiResponse<object>.Fail(message, 500, ex.Message), statusCode: 500);
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
