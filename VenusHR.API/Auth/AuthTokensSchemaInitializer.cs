using Microsoft.EntityFrameworkCore;
using VenusHR.Infrastructure.Presistence;

namespace VenusHR.API.Auth
{
    public class AuthTokensSchemaInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuthTokensSchemaInitializer> _logger;

        public AuthTokensSchemaInitializer(
            IServiceScopeFactory scopeFactory,
            ILogger<AuthTokensSchemaInitializer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            const string createTablesSql = @"
IF OBJECT_ID(N'dbo.UserActiveTokens', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserActiveTokens](
        [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [EmployeeId] [int] NOT NULL,
        [Jti] [nvarchar](100) NOT NULL,
        [IssuedAt] [datetime2] NOT NULL
    );

    CREATE UNIQUE INDEX [IX_UserActiveTokens_EmployeeId]
    ON [dbo].[UserActiveTokens] ([EmployeeId]);
END;

IF OBJECT_ID(N'dbo.RevokedTokens', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RevokedTokens](
        [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Token] [nvarchar](1000) NOT NULL,
        [RevokedAt] [datetime2] NOT NULL,
        [ExpiresAt] [datetime2] NULL,
        [Reason] [nvarchar](100) NULL
    );
END;";

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                await dbContext.Database.ExecuteSqlRawAsync(createTablesSql, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize auth tokens schema.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
