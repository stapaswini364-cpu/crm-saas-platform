using Serilog;

using CRM.API.Middleware;
using CRM.API.Authorization;

using CRM.Application.Interfaces;
using CRM.Application.Common.Security;

using CRM.Infrastructure.Data;
using CRM.Infrastructure.Security;
using CRM.Infrastructure.Services;
using HealthChecks.Redis;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Microsoft.IdentityModel.Tokens;

using RabbitMQ.Client;

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
// Controllers
// =====================

builder.Services.AddControllers();


// =====================
// Swagger
// =====================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// =====================
// Database
// =====================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// =====================
// Redis Cache
// =====================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "CRM:";
});


// =====================
// Health Checks
// =====================

builder.Services
    .AddHealthChecks()

    // PostgreSQL
    .AddDbContextCheck<ApplicationDbContext>()

    // Redis
    .AddRedis(
        "localhost:6379",
        name: "redis")

    // RabbitMQ
    .AddRabbitMQ(
        sp =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(
                    "amqp://admin:admin123@localhost:5672/")
            };

            return factory.CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();
        },
        name: "rabbitmq");


// =====================
// Dependency Injection
// =====================

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

builder.Services.AddScoped<RefreshTokenService>();

builder.Services.AddScoped<
    ITenantContextAccessor,
    TenantContextAccessor>();


// =====================
// Dashboard Service
// =====================

builder.Services.AddScoped<
    IDashboardService,
    DashboardService>();


// =====================
// Redis Cache Service
// =====================

builder.Services.AddScoped<
    IRedisCacheService,
    RedisCacheService>();


// =====================
// Authentication JWT
// =====================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
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
                            builder.Configuration["Jwt:Key"]!))
            };
    });


// =====================
// Authorization RBAC
// =====================

builder.Services.AddSingleton<
    IAuthorizationHandler,
    PermissionHandler>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "Users.Create",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.UsersCreate)));


    options.AddPolicy(
        "Users.Update",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.UsersUpdate)));


    options.AddPolicy(
        "Users.Delete",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.UsersDelete)));


    options.AddPolicy(
        "Organizations.Create",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.OrganizationsCreate)));


    options.AddPolicy(
        "Organizations.Update",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.OrganizationsUpdate)));


    options.AddPolicy(
        "Organizations.Delete",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.OrganizationsDelete)));


    options.AddPolicy(
        "Reports.View",
        policy =>
            policy.Requirements.Add(
                new PermissionRequirement(
                    Permissions.ReportsView)));
});


// =====================
// Build
// =====================

var app = builder.Build();


// =====================
// Serilog Request Logging
// =====================

app.UseSerilogRequestLogging();


// =====================
// Swagger Middleware
// =====================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// =====================
// Middleware Pipeline
// =====================

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<TenantResolutionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();


// =====================
// Endpoints
// =====================

app.MapControllers();

app.MapHealthChecks("/health");


app.Run();