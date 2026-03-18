using ConexyTask.DbContext;
using ConexyTask.Repository;
using ConexyTask.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portEnv) && int.TryParse(portEnv, out var port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var rawConn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(rawConn) &&
    (rawConn.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
     rawConn.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
{
    var uri = new Uri(rawConn);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var database = uri.AbsolutePath.TrimStart('/');

    var normalized =
        $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};Ssl Mode=Require;Trust Server Certificate=true";

    builder.Configuration["ConnectionStrings:DefaultConnection"] = normalized;
}


builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<DbConexy>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public")));

builder.Services.AddScoped<IConexyRepository, ConexyRepository>();
builder.Services.AddScoped<IConexyService, ConexyService>();
var app = builder.Build();

var environment = app.Environment.EnvironmentName;
if (environment == "Production" || environment == "Docker")
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
            logger.LogError(ex, "Произошла ошибка при применении миграций базы данных");
            // В Production/Docker не продолжаем работу без БД
            if (environment == "Production")
            {
                throw;
            }
        }
    }
}

app.UseCors();
app.MapControllers();
app.Run();