using Serilog;
using CRM.API.Middleware;
using CRM.API.Services;
using CRM.Application.Interfaces;
using CRM.Application.Services;
using CRM.Infrastructure.Security;
using CRM.Infrastructure.Services;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
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