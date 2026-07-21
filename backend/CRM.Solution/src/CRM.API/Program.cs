using Serilog;

using CRM.API.Middleware;
using CRM.API.Authorization;

using CRM.Application.Interfaces;
using CRM.Application.Common.Security;

using CRM.Infrastructure.Data;
using CRM.Infrastructure.Security;
using CRM.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Microsoft.IdentityModel.Tokens;

using System.Text;


var builder = WebApplication.CreateBuilder(args);


// =====================
// Serilog
// =====================

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();


// =====================
// Services
// =====================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// Health Check

builder.Services.AddHealthChecks();


// =====================
// Database
// =====================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration
        .GetConnectionString("DefaultConnection"));
});


// =====================
// Dependency Injection
// =====================

builder.Services.AddScoped<
    IJwtTokenService,
    JwtTokenService>();


builder.Services.AddScoped<
    IPasswordHasher,
    BCryptPasswordHasher>();


builder.Services.AddScoped<RefreshTokenService>();


builder.Services.AddScoped<
    ITenantContextAccessor,
    TenantContextAccessor>();




// =====================
// Authentication
// =====================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,


                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],


                ValidAudience =
                    builder.Configuration["Jwt:Audience"],


                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        ))
            };
    });




// =====================
// Authorization RBAC
// =====================

builder.Services.AddSingleton<
    IAuthorizationHandler,
    PermissionHandler>();


builder.Services.AddAuthorization();



// =====================
// Build
// =====================

var app = builder.Build();



// =====================
// Swagger
// =====================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}



// =====================
// Middleware
// =====================

app.UseHttpsRedirection();


app.UseMiddleware<GlobalExceptionMiddleware>();


app.UseMiddleware<TenantResolutionMiddleware>();


app.UseAuthentication();


app.UseAuthorization();



// =====================
// Routes
// =====================

app.MapHealthChecks("/api/Health");


app.MapControllers();


app.Run();


// Required for WebApplicationFactory tests
public partial class Program
{
}