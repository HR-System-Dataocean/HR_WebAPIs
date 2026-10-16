using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Infrastructure.Services;
using VenusHR.API.Models;
using System.ComponentModel.DataAnnotations;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
         app.MapPost("/api/auth/login", Login).AllowAnonymous();
        app.MapGet("/api/auth/status", Status).RequireAuthorization();
        app.MapPost("/api/auth/logout", Logout).RequireAuthorization();
        app.MapPost("/api/auth/change-password", ChangePassword).RequireAuthorization();
    }

    private static IResult ChangePassword(
        [FromBody] ChangePasswordRequest request,
        HttpContext httpContext,
        ILoginServices loginService,
        [FromQuery] int lang = 2)
    {
        if (request == null)
        {
            var message = lang == 1 ? "بيانات الطلب غير صحيحة" : "Invalid request data";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        var username = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(username))
        {
            var message = lang == 1 ? "المستخدم غير مصرح له" : "Unauthorized user";
            return Results.Json(ApiResponse.Fail(message), statusCode: 401);
        }

        try
        {
            var result = loginService.ChangePassword(
                username,
                request.CurrentPassword,
                request.NewPassword,
                request.ConfirmPassword,
                lang);

            if (result is WorkFlow_EF.GeneralOutputClass<object> output)
            {
                if (output.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل تغيير كلمة المرور" : "Change password failed";
                    return Results.BadRequest(ApiResponse<object>.Fail(output.ErrorMessage ?? message, output.ErrorCode));
                }

                return Results.Ok(ApiResponse<object>.Ok(
                    null,
                    output.ErrorMessage ?? (lang == 1 ? "تم تغيير كلمة المرور بنجاح" : "Password changed successfully")));
            }

            var errorMsg = lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
            return Results.Json(ApiResponse<object>.Fail(errorMsg), statusCode: 500);
        }
        catch (Exception ex)
        {
            var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
            return Results.Json(ApiResponse<object>.Fail(message, 500, ex.Message), statusCode: 500);
        }
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromQuery] int lang,
        ILoginServices loginService)
    {
        // Validation
        if (request == null)
        {
            var message = lang == 1 ? "بيانات الطلب غير صحيحة" : "Invalid request data";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            var message = lang == 1 ? "اسم المستخدم مطلوب" : "Username is required";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            var message = lang == 1 ? "كلمة المرور مطلوبة" : "Password is required";
            return Results.BadRequest(ApiResponse.Fail(message));
        }

        try
        {
            var result = loginService.Login(
                request.Code,
                request.Password,
                lang,
                request.DeviceToken);

            if (result is WorkFlow_EF.GeneralOutputClass<object> output)
            {
                if (output.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل تسجيل الدخول" : "Login failed";
                    // Return 400 Bad Request for all login failures
                    return Results.BadRequest(ApiResponse<object>.Fail(
                        output.ErrorMessage ?? message,
                        output.ErrorCode));
                }

                return Results.Ok(ApiResponse<object>.Ok(
                    output.ResultObject,
                    output.ErrorMessage ?? (lang == 1 ? "تم تسجيل الدخول بنجاح" : "Login successful")));
            }

            var errorMsg = lang == 1 ? "حدث خطأ غير متوقع" : "An unexpected error occurred";
            return Results.Json(ApiResponse<object>.Fail(errorMsg), statusCode: 500);
        }
        catch (Exception ex)
        {
            var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
            return Results.Json(ApiResponse<object>.Fail(message, 500, ex.Message), statusCode: 500);
        }
    }

    private static IResult Status()
    {
        return Results.Ok(ApiResponse.Ok("API is running"));
    }

    private static async Task<IResult> Logout(HttpContext httpContext, IJwtService jwtService)
    {
        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            await jwtService.RevokeTokenAsync(token, "User logout");
        }
        
        return Results.Ok(ApiResponse.Ok("Logged out successfully"));
    }
}

public class LoginRequest
{
    [Required(ErrorMessage = "Username is required")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public string? DeviceToken { get; set; }
}

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
