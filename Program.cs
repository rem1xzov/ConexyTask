using ConexyTask.DbContext;
using ConexyTask.Repository;
using ConexyTask.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Подключение к базе
var rawConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
              ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(rawConn) && (rawConn.StartsWith("postgres://") || rawConn.StartsWith("postgresql://")))
{
    var uri = new Uri(rawConn);
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";
    var host = uri.Host;
    var port = uri.Port;
    var database = uri.AbsolutePath.TrimStart('/');

    rawConn = $"Host={host};Port={port};Database={database};Username={username};Password={password};Ssl Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Настройка БД без лишней вложенности
builder.Services.AddDbContext<DbConexy>(options =>
    options.UseNpgsql(rawConn, npgsqlOptions => 
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
    }));

builder.Services.AddScoped<IConexyRepository, ConexyRepository>();
builder.Services.AddScoped<IConexyService, ConexyService>();

var app = builder.Build();

// Миграции
if (app.Environment.IsDevelopment()  app.Environment.EnvironmentName == "Production"  app.Environment.EnvironmentName == "Docker")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<DbConexy>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ошибка миграции");
        }
    }
}

app.UseCors();
app.MapControllers();
app.Run();
