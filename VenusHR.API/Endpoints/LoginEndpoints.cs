using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Core.Login;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
         app.MapPost("/api/auth/login", Login);
        app.MapGet("/api/auth/status", Status);
        app.MapPost("/api/auth/logout", Logout);
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

    private static IResult Logout()
    {
        return Results.Ok(new { Message = "Logged out" });
    }
}

  