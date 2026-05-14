using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;
using VenusHR.Infrastructure.Services;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
         app.MapPost("/api/auth/login", Login).AllowAnonymous();
        app.MapGet("/api/auth/status", Status).RequireAuthorization();
        app.MapPost("/api/auth/logout", Logout).RequireAuthorization();
    }

    private static async Task<IResult> Login(
        [FromBody] SysUser user,
        [FromQuery] int lang,
        ILoginServices loginService)
    {
        var result = loginService.Login(
            user.Code,
            user.Password,
            lang,
            user.DeviceToken);
        return Results.Ok(result);
    }

    private static IResult Status()
    {
        return Results.Ok(new { Status = "API is running" });
    }

    private static async Task<IResult> Logout(HttpContext httpContext, IJwtService jwtService)
    {
        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            await jwtService.RevokeTokenAsync(token, "User logout");
        }
        
        return Results.Ok(new { Message = "Logged out successfully" });
    }
}