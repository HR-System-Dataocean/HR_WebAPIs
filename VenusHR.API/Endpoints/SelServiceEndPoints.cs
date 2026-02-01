using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.SelfService;
using VenusHR.Infrastructure.Presistence;
using System.Dynamic;

public static class SelfServiceEndpoints
{
    public static void MapSelfServiceEndpoints(this WebApplication app)
    {
        // 🔹 1. Annual Vacation Requests
        app.MapPost("/api/self-service/vacation/save-request", SaveRequest);
        app.MapGet("/api/self-service/vacation/balances/{empId:int}/{toDate:datetime}", GetAnnualVacsBalancesByEMP);
        app.MapGet("/api/self-service/vacation/service-period/{employeeId:int}/{endServiceDate:datetime}", GeatServicePeriod);
        app.MapGet("/api/self-service/vacation/stages/{requestSerial:int}/{formCode}/{lang:int}", GetRequestStages);
        app.MapGet("/api/self-service/vacation/request-details/{requestType}/{requestId:int}/{lang:int}/{configId:int}", GetSelfServiceRequestDetails);
        app.MapGet("/api/self-service/vacation/my-request-details/{requestType}/{requestId:int}/{lang:int}", GetMySelfServiceRequestDetails);
        app.MapGet("/api/self-service/vacation/request/{requestSerial:int}/{lang:int}", GetAnnualVacationRequestByID);
        app.MapGet("/api/self-service/vacation/employee-requests/{employeeId:int}/{lang:int}", GetEmployeeRequests);

        // 🔹 2. Master Endpoints (من SMaster Controller)
        app.MapGet("/api/self-service/master/request-types", GetAllRequestTypes);
        app.MapGet("/api/self-service/master/employees/{lang:int}", GetAllEmployees);
        app.MapGet("/api/self-service/master/employee/{id:int}/{lang:int}", GetEmployeeByID);
        app.MapGet("/api/self-service/master/notifications/count/{employeeId}", GetUserNotificationCount);
        app.MapGet("/api/self-service/master/vacation-types/{lang:int}", GetAllVacationsTypes);
        app.MapGet("/api/self-service/master/resignation-reasons/{lang:int}", GetEndOfServiceAllResignationReason);
        app.MapGet("/api/self-service/master/experience-rates/{lang:int}", GetEndOfServiceAllExperienceRate);
        app.MapGet("/api/self-service/master/pending-requests/{employeeId:int}/{lang:int}", GetAllPendingRequests);
        app.MapPost("/api/self-service/master/request-action", SaveRequestAction);

        // 🔹 3. إضافة Endpoints إضافية لو عندك
        // app.MapGet("/api/self-service/leaves/types", GetAllLeavesTypes);
        // app.MapPost("/api/self-service/requests/submit", SubmitRequest);
    }

    // =========== Vacation Requests Methods ===========

    private static async Task<IResult> SaveRequest(
        [FromBody] dynamic requestData,
        [FromQuery] string requestType,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.SaveSelfServiceRequest(requestData, requestType);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAnnualVacsBalancesByEMP(
        int empId,
        DateTime toDate,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GetAnnualVacsBalancesByEMP(empId, toDate);
        return Results.Ok(result);
    }

    private static async Task<IResult> GeatServicePeriod(
        int employeeId,
        DateTime endServiceDate,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GeatServicePeriod(employeeId, endServiceDate);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRequestStages(
        int requestSerial,
        string formCode,
        int lang,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GetRequestStages(requestSerial, formCode, lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetSelfServiceRequestDetails(
        string requestType,
        int requestId,
        int lang,
        int configId,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GetSelfServiceRequestDetails(requestType, requestId, lang, configId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMySelfServiceRequestDetails(
        string requestType,
        int requestId,
        int lang,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GetMySelfServiceRequestDetails(requestType, requestId, lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAnnualVacationRequestByID(
        int requestSerial,
        int lang,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GetAnnualVacationRequestByID(requestSerial, lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEmployeeRequests(
        int employeeId,
        int lang,
        [FromServices] IAnnualVacationRequestService service)
    {
        var result = service.GetEmployeeRequests(employeeId, lang);
        return Results.Ok(result);
    }

    // =========== Master Methods (من SMaster Controller) ===========

    private static async Task<IResult> GetAllRequestTypes(
        [FromServices] IMaster service)
    {
        var result = service.GetAllRequestTypes();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllEmployees(
        int lang,
        [FromServices] IMaster service)
    {
        var result = service.GetAllEmployees(lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEmployeeByID(
        int id,
        int lang,
        [FromServices] IMaster service)
    {
        var result = service.GetEmployeeByID(id, lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserNotificationCount(
        string employeeId,
        [FromServices] IMaster service)
    {
        var result = service.GetUserNotificationCount(employeeId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllVacationsTypes(
        int lang,
        [FromServices] IMaster service)
    {
        var result = service.GetAllVacationsTypes(lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEndOfServiceAllResignationReason(
        int lang,
        [FromServices] IMaster service)
    {
        var result = service.GetEndOfServiceAllResignationReason(lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEndOfServiceAllExperienceRate(
        int lang,
        [FromServices] IMaster service)
    {
        var result = service.GetEndOfServiceAllExperienceRate(lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllPendingRequests(
        int employeeId,
        int lang,
        [FromServices] IMaster service)
    {
        var result = service.GetAllPendingRequests(employeeId, lang);
        return Results.Ok(result);
    }

    private static async Task<IResult> SaveRequestAction(
        [FromBody] SS_RequestAction requestAction,
        [FromServices] IMaster service)
    {
        var result = service.SaveRequestAction(requestAction);
        return Results.Ok(result);
    }
}