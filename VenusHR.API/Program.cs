using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using VenusHR.Application.Common.Interfaces;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Application.Common.Interfaces.Documents;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Application.Common.Interfaces.Login;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Infrastructure.Services;
using VenusHR.Core.Master;
using VenusHR.Infrastructure;
using VenusHR.Infrastructure.Presistence;
using VenusHR.Infrastructure.Presistence.Attendance;
using VenusHR.Infrastructure.Presistence.HRServices;
using VenusHR.Infrastructure.Presistence.Login;
using VenusHR.Infrastructure.Presistence.SelfService;
using VenusHR.Infrastructure.Services.Documents;
using YourNamespace;
using VenusHR.API.Hubs;
using VenusHR.API.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpClient(nameof(FcmPushNotificationSender));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyMethod()  
              .AllowAnyHeader();  
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VenusHR API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Include XML comments
    try
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch
    {
        // Ignore XML comments errors
    }

    // Configure schema generation to be more lenient
    c.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
});
//Rabie
//Dependency Injection
builder.Services.AddPersistence(builder.Configuration);


// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourDefaultSecretKeyThatShouldBeAtLeast32CharactersLong!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "VenusHR",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "VenusHRUsers",
        // Lifetime is controlled by UserActiveTokens (logout / new login), not by exp time.
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrWhiteSpace(accessToken) && path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        },
        // Reject tokens that are no longer the user's current active token
        // (user logged out, or logged in again and got a new token).
        OnTokenValidated = async context =>
        {
            var jti = context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;
            var employeeIdClaim = context.Principal?.FindFirst("EmployeeId")?.Value;

            if (jti == null || !int.TryParse(employeeIdClaim, out var employeeId))
            {
                context.Fail("Invalid token.");
                return;
            }

            var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDBContext>();
            var isActive = await db.UserActiveTokens
                .AnyAsync(t => t.EmployeeId == employeeId && t.Jti == jti);

            if (!isActive)
            {
                context.Fail("Token has been revoked. Please log in again.");
            }
        }
    };
});

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMaster, MasterService>();
builder.Services.AddScoped<IHRMaster, HRMasreService>();
builder.Services.AddScoped<IAnnualVacationRequestService, AnnualVacationRequestSevice>();
builder.Services.AddScoped<IAdvanceHousingRequestService, AdvanceHousingRequestService>();
builder.Services.AddScoped<IAdvanceSalaryRequestService, AdvanceSalaryRequestService>();
builder.Services.AddScoped<IAnnualTicketRelatedRequestsService, AnnualTicketRelatedRequestsService>();
builder.Services.AddScoped<IAssaultEscalationFormRequestService, AssaultEscalationFormRequestService>();
builder.Services.AddScoped<IBankAccountUpdateService, BankAccountUpdateService>();
builder.Services.AddScoped<IBusinessOrtrainingTravelService, BusinessOrtrainingTravelService>();
builder.Services.AddScoped<IChamberofCommerceLetterRequestService, ChamberofCommerceLetterRequestService>();
builder.Services.AddScoped<IChangeWorkHoursRequestService, ChangeWorkHoursRequestService>();
builder.Services.AddScoped<IConflictofInterestFormRequestService, ConflictofInterestFormRequestService>();
builder.Services.AddScoped<IContactInformationUpdateService, ContactInformationUpdateService>();
builder.Services.AddScoped<IDaycareSupportReaquestService, DaycareSupportReaquestService>();
builder.Services.AddScoped<IDependentsInformationUpdateService, DependentsInformationUpdateService>();
builder.Services.AddScoped<IEducationFeesCompensationApplicationService, EducationFeesCompensationApplicationService>();
builder.Services.AddScoped<IEducationSupportRequestService, EducationSupportRequestService>();
builder.Services.AddScoped<IEmployeeFileUpdateService, EmployeeFileUpdateService>();
builder.Services.AddScoped<IEndOfServiceRequestService, EndOfServiceRequestService>();
builder.Services.AddScoped<IExecuseRequestService, ExecuseRequestService>();
builder.Services.AddScoped<IExitEntryRequestService, ExitEntryRequestService>();
builder.Services.AddScoped<IGrievanceFormRequestService, GrievanceFormRequestService>();
builder.Services.AddScoped<IInterviewEvaluationFormRequestService, InterviewEvaluationFormRequestService>();
builder.Services.AddScoped<ILoanLetterRequestService, LoanLetterRequestService>();
builder.Services.AddScoped<IMedicalInsuranceAdjustmentsService, MedicalInsuranceAdjustmentsService>();
builder.Services.AddScoped<IOccurrenceVarianceReportingLetterService, OccurrenceVarianceReportingLetterService>();
builder.Services.AddScoped<IOtherLegalDocumentUpdatesService, OtherLegalDocumentUpdatesService>();
builder.Services.AddScoped<IOtherLetterRequestService, OtherLetterRequestService>();
builder.Services.AddScoped<IOvertimeRequestService, OvertimeRequestService>();
builder.Services.AddScoped<IPaySlipRequestService, PaySlipRequestService>();
builder.Services.AddScoped<IPhysiciansPrivilegingFormRequestService, PhysiciansPrivilegingFormRequestService>();
builder.Services.AddScoped<ILoginServices, LoginServices>();
builder.Services.AddScoped<IAttendance, AttendanceSercives>();
builder.Services.AddScoped<IDocumentsService, DocumentsService>();
builder.Services.AddSingleton<IUserIdProvider, EmployeeIdUserIdProvider>();
builder.Services.AddSingleton<IOnlineUserTracker, OnlineUserTracker>();
builder.Services.AddScoped<IFcmPushNotificationSender, FcmPushNotificationSender>();
builder.Services.AddHostedService<VenusHR.API.Auth.AuthTokensSchemaInitializer>();
builder.Services.AddHostedService<AttendanceNotificationsSchemaInitializer>();
builder.Services.AddHostedService<AttendanceNotificationWorker>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AttendanceNotificationHub>("/hubs/notifications");

//MinimalAPI's
app.MapLoginEndpoints();
app.MapSelfServiceEndpoints();
app.MapAttendanceEndpoints();
app.MapHRMasterEndpoints();

app.Run();
