using Microsoft.AspNetCore.SignalR;

namespace VenusHR.API.Notifications
{
    public class EmployeeIdUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Claims?.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
        }
    }
}
