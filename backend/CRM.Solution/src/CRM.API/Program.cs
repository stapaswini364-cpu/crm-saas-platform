using Serilog;
using CRM.API.Middleware;
using CRM.API.Services;
using CRM.Application.Common.Security;
using CRM.Application.Interfaces;
using CRM.Application.Services;
using CRM.Infrastructure.Security;
using CRM.Infrastructure.Services;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CRM.API.Authorization;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Configure Serilog
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
// Database
// =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=faatpro_db;Username=admin;Password=password"
    ));


// =====================
// Dependency Injection
// =====================
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();


// =====================
// JWT Authentication
// =====================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!
            )
        )
    };
});


// =====================
// Authorization (RBAC + Permissions)
// =====================

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Users.Create", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.UsersCreate)));

    options.AddPolicy("Users.Update", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.UsersUpdate)));

    options.AddPolicy("Users.Delete", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.UsersDelete)));

    options.AddPolicy("Organizations.Create", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.OrganizationsCreate)));

    options.AddPolicy("Organizations.Update", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.OrganizationsUpdate)));

    options.AddPolicy("Organizations.Delete", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.OrganizationsDelete)));

    options.AddPolicy("Reports.View", policy =>
        policy.Requirements.Add(
            new PermissionRequirement(Permissions.ReportsView)));
});


// =====================
// OpenAPI
// =====================
builder.Services.AddOpenApi();


var app = builder.Build();


// =====================
// Development
// =====================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// =====================
// HTTP Pipeline
// =====================
app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<TenantResolutionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();