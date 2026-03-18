using ConexyTask.DbContext;
using ConexyTask.Repository;
using ConexyTask.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Получаем строку подключения (сначала из Render, потом из конфига)
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Парсим строку, если она в формате postgres:// (специально для Render)
if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    var user = userInfo[0];
    var pass = userInfo.Length > 1 ? userInfo[1] : "";
    
    // ФИКС: Если порт не указан (-1), ставим стандартный 5432
    var port = uri.Port <= 0 ? 5432 : uri.Port;
    var host = uri.Host;
    var db = uri.AbsolutePath.TrimStart('/');

    connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass};Ssl Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 3. Подключаем базу данных
builder.Services.AddDbContext<DbConexy>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IConexyRepository, ConexyRepository>();
builder.Services.AddScoped<IConexyService, ConexyService>();

var app = builder.Build();

// 4. Применяем миграции при старте (автоматическое создание таблиц)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DbConexy>();
        context.Database.Migrate();
        Console.WriteLine("Миграции успешно применены.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при миграции: {ex.Message}");
        // Не валим приложение сразу, чтобы можно было посмотреть логи
    }
}

app.UseCors();
app.MapControllers();

app.Run();
