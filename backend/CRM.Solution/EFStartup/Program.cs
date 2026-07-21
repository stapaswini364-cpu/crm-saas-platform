using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("EF Startup");

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

optionsBuilder.UseNpgsql(
    "Host=localhost;Port=5432;Database=faatpro_db;Username=admin;Password=password"
);

using var db = new ApplicationDbContext(optionsBuilder.Options);