using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VenusHR.API.Notifications;

namespace VenusHR.API.Hubs
{
    [Authorize]
    public class AttendanceNotificationHub : Hub
    {
        private readonly IOnlineUserTracker _onlineUserTracker;

        public AttendanceNotificationHub(IOnlineUserTracker onlineUserTracker)
        {
            _onlineUserTracker = onlineUserTracker;
        }

        public override Task OnConnectedAsync()
        {
            var employeeId = GetEmployeeId();
            if (employeeId.HasValue)
            {
                _onlineUserTracker.AddConnection(employeeId.Value, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var employeeId = GetEmployeeId();
            if (employeeId.HasValue)
            {
                _onlineUserTracker.RemoveConnection(employeeId.Value, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        private int? GetEmployeeId()
        {
            var employeeClaim = Context.User?.Claims?.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            if (int.TryParse(employeeClaim, out var employeeId))
            {
                return employeeId;
            }

            return null;
        }
    }
}
