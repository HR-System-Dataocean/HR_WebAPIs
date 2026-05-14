using Microsoft.AspNetCore.Cors.Infrastructure;
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
using VenusHR.Infrastructure.Presistence.Attendance;
using VenusHR.Infrastructure.Presistence.HRServices;
using VenusHR.Infrastructure.Presistence.Login;
using VenusHR.Infrastructure.Presistence.SelfService;
using VenusHR.Infrastructure.Services.Documents;
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMaster, MasterService>();
builder.Services.AddScoped<IHRMaster, HRMasreService>();
builder.Services.AddScoped<IAnnualVacationRequestService, AnnualVacationRequestSevice>();
builder.Services.AddScoped<ILoginServices, LoginServices>();
builder.Services.AddScoped<IAttendance, AttendanceSercives>();
builder.Services.AddScoped<IDocumentsService, DocumentsService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//MinimalAPI's
app.MapLoginEndpoints();
app.MapSelfServiceEndpoints();
app.MapAttendanceEndpoints();
app.MapHRMasterEndpoints();

app.Run();
