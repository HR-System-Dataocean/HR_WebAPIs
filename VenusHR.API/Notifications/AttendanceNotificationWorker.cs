using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VenusHR.API.Hubs;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Core.Notifications;
using VenusHR.Infrastructure.Presistence;

namespace VenusHR.API.Notifications
{
    public class AttendanceNotificationWorker : BackgroundService
    {
        private const string NotificationType = "AttendanceAssignment";

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOnlineUserTracker _onlineUserTracker;
        private readonly IHubContext<AttendanceNotificationHub> _hubContext;
        private readonly ILogger<AttendanceNotificationWorker> _logger;

        public AttendanceNotificationWorker(
            IServiceScopeFactory scopeFactory,
            IOnlineUserTracker onlineUserTracker,
            IHubContext<AttendanceNotificationHub> hubContext,
            ILogger<AttendanceNotificationWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _onlineUserTracker = onlineUserTracker;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ProcessAttendanceNotificationsAsync(stoppingToken);

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessAttendanceNotificationsAsync(stoppingToken);
            }
        }

        private async Task ProcessAttendanceNotificationsAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var attendance = scope.ServiceProvider.GetRequiredService<IAttendance>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var fcmSender = scope.ServiceProvider.GetRequiredService<IFcmPushNotificationSender>();

                var now = DateTime.Now;
                var today = now.Date;
                var dueEmployees = attendance.GetExpectedStartEmployeesBeforeTime(now.Hour, now.Minute);
                if (!dueEmployees.Any())
                {
                    return;
                }

                foreach (var employee in dueEmployees)
                {
                    var alreadyNotified = await dbContext.AttendanceNotifications.AnyAsync(
                        n => n.EmployeeId == employee.EmployeeId
                             && n.NotificationDate == today
                             && n.NotificationType == NotificationType,
                        cancellationToken);

                    if (alreadyNotified)
                    {
                        continue;
                    }

                    var title = "Assign attendance";
                    var body = $"You did not assign attendance today. Expected start time: {employee.AllowedStartTime}.";

                    var notification = new AttendanceNotification
                    {
                        EmployeeId = employee.EmployeeId,
                        NotificationDate = today,
                        NotificationType = NotificationType,
                        Title = title,
                        Body = body,
                        DeliveryStatus = "Pending",
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.AttendanceNotifications.Add(notification);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    if (_onlineUserTracker.IsOnline(employee.EmployeeId))
                    {
                        await _hubContext.Clients.User(employee.EmployeeId.ToString()).SendAsync(
                            "attendanceReminder",
                            new
                            {
                                notification.Id,
                                notification.Title,
                                notification.Body,
                                notification.NotificationDate,
                                notification.NotificationType
                            },
                            cancellationToken);

                        notification.SentViaSignalR = true;
                        notification.DeliveryStatus = "SentViaSignalR";
                        notification.SentAt = DateTime.UtcNow;
                    }
                    else
                    {
                        var tokens = await dbContext.Sys_Users
                            .Where(x => x.RelEmployee == employee.EmployeeId
                                        && x.CancelDate == null
                                        && x.DeviceToken != null
                                        && x.DeviceToken != string.Empty)
                            .Select(x => x.DeviceToken!)
                            .ToListAsync(cancellationToken);

                        var data = new Dictionary<string, string>
                        {
                            ["employeeId"] = employee.EmployeeId.ToString(),
                            ["notificationType"] = NotificationType
                        };

                        var sent = await fcmSender.SendAsync(tokens, title, body, data, cancellationToken);
                        notification.SentViaFcm = sent;
                        notification.DeliveryStatus = sent ? "SentViaFcm" : "NoDeviceTokenOrFcmFailed";
                        notification.SentAt = sent ? DateTime.UtcNow : null;
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing attendance notifications.");
            }
        }
    }
}
