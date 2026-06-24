using Microsoft.EntityFrameworkCore;
using VenusHR.Infrastructure.Presistence;

namespace VenusHR.API.Notifications
{
    public class AttendanceNotificationsSchemaInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AttendanceNotificationsSchemaInitializer> _logger;

        public AttendanceNotificationsSchemaInitializer(
            IServiceScopeFactory scopeFactory,
            ILogger<AttendanceNotificationsSchemaInitializer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            const string createTableSql = @"
IF OBJECT_ID(N'dbo.Hrs_AttendanceNotifications', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Hrs_AttendanceNotifications](
        [Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [EmployeeId] [int] NOT NULL,
        [NotificationDate] [datetime2] NOT NULL,
        [NotificationType] [nvarchar](100) NOT NULL,
        [Title] [nvarchar](300) NOT NULL,
        [Body] [nvarchar](2000) NOT NULL,
        [SentViaSignalR] [bit] NOT NULL CONSTRAINT [DF_Hrs_AttendanceNotifications_SentViaSignalR] DEFAULT(0),
        [SentViaFcm] [bit] NOT NULL CONSTRAINT [DF_Hrs_AttendanceNotifications_SentViaFcm] DEFAULT(0),
        [DeliveryStatus] [nvarchar](50) NOT NULL,
        [CreatedAt] [datetime2] NOT NULL,
        [SentAt] [datetime2] NULL
    );
END;

IF COL_LENGTH(N'dbo.Hrs_AttendanceNotifications', N'SentViaFcm') IS NULL
BEGIN
    ALTER TABLE [dbo].[Hrs_AttendanceNotifications]
    ADD [SentViaFcm] [bit] NOT NULL CONSTRAINT [DF_Hrs_AttendanceNotifications_SentViaFcm] DEFAULT(0);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Hrs_AttendanceNotifications_EmployeeId_NotificationDate_NotificationType'
      AND object_id = OBJECT_ID(N'dbo.Hrs_AttendanceNotifications')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Hrs_AttendanceNotifications_EmployeeId_NotificationDate_NotificationType]
    ON [dbo].[Hrs_AttendanceNotifications] ([EmployeeId], [NotificationDate], [NotificationType]);
END;";

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                await dbContext.Database.ExecuteSqlRawAsync(createTableSql, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize attendance notifications schema.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
