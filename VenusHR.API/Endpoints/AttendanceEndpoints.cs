using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Core.Master;
using WorkFlow_EF;

public static class AttendanceEndpoints
{
    public static void MapAttendanceEndpoints(this WebApplication app)
    {
        // Check In/Out
        app.MapPost("/api/attendance/check-in-out", CheckInOut);

        // Get Attendance History
        app.MapGet("/api/attendance/history", GetAttendanceHistory);

        // Daily Summary
        app.MapGet("/api/attendance/daily-summary", GetDailySummary);

        // Monthly Report
        app.MapGet("/api/attendance/monthly-report", GetMonthlyReport);

        // Employee Attendance
        app.MapGet("/api/attendance/employee/{employeeId}", GetEmployeeAttendance);
    }

    // =========== Check In/Out ===========

    private static async Task<IResult> CheckInOut(
        [FromBody] Hrs_Mobile_Attendance request,
        [FromQuery] int lang,
        [FromQuery] string checkType,
        [FromServices] IAttendance attendanceService)
    {
        try
        {
            if (request == null)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "بيانات الطلب غير صحيحة" : "Invalid request data"
                });
            }

            var result = attendanceService.CheckInOut(
                request.EmployeeID,
                request.Latitude,
                request.Longitude,
                request.CheckingDatetime,
                request.DeviceID,
                request.DeviceModel,
                request.OSVersion,
                request.NetworkType,
                lang,
                checkType
            );

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: (lang == 1) ? "حدث خطأ في الخادم" : "Server error occurred",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error");
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
        try
        {
            if (employeeID <= 0)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                });
            }

            var result = attendanceService.GetAttendanceHistory(employeeID, fromDate, toDate);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: (lang == 1) ? "حدث خطأ في جلب سجل الحضور" : "Error retrieving attendance history",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error");
        }
    }

    // =========== Daily Summary ===========

    private static async Task<IResult> GetDailySummary(
        [FromQuery] int employeeID,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        try
        {
            if (employeeID <= 0)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                });
            }

            var result = attendanceService.GetAttendanceHistory(employeeID, fromDate, toDate);

            if (result is GeneralOutputClass<object> output && output.ErrorCode == 0)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = output.ErrorMessage
                });
            }

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: (lang == 1) ? "حدث خطأ في جلب الملخص اليومي" : "Error retrieving daily summary",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error");
        }
    }

    // =========== Monthly Report ===========

    private static async Task<IResult> GetMonthlyReport(
        [FromQuery] int employeeID,
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        try
        {
            if (employeeID <= 0)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                });
            }

            if (year < 2000 || year > DateTime.Now.Year + 1)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "السنة غير صحيحة" : "Invalid year"
                });
            }

            if (month < 1 || month > 12)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "الشهر غير صحيح" : "Invalid month"
                });
            }

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var result = attendanceService.GetAttendanceHistory(employeeID, startDate, endDate);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: (lang == 1) ? "حدث خطأ في جلب التقرير الشهري" : "Error retrieving monthly report",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error");
        }
    }

    // =========== Employee Attendance ===========

    private static async Task<IResult> GetEmployeeAttendance(
        int employeeId,
        [FromQuery] int lang,
        [FromServices] IAttendance attendanceService)
    {
        try
        {
            if (employeeId <= 0)
            {
                return Results.BadRequest(new
                {
                    Status = false,
                    Message = (lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID"
                });
            }

            var fromDate = DateTime.Now.AddDays(-30);
            var result = attendanceService.GetAttendanceHistory(employeeId, fromDate, DateTime.Now);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: (lang == 1) ? "حدث خطأ في جلب سجل الموظف" : "Error retrieving employee attendance",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error");
        }
    }
}