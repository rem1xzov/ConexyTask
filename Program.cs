using ConexyTask.DbContext;
using ConexyTask.Repository;
using ConexyTask.Service;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var rawConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
              ?? builder.Configuration.GetConnectionString("DefaultConnection");

string finalConn;

if (!string.IsNullOrEmpty(rawConn) && rawConn.Contains("://"))
{
    var builderConn = new NpgsqlConnectionStringBuilder();
    var uri = new Uri(rawConn);
    
    builderConn.Host = uri.Host;
    builderConn.Database = uri.AbsolutePath.TrimStart('/');
    
    var userInfo = uri.UserInfo.Split(':');
    builderConn.Username = userInfo[0];
    if (userInfo.Length > 1) builderConn.Password = userInfo[1];

    if (uri.Port > 0) 
    {
        builderConn.Port = uri.Port;
    }

    builderConn.SslMode = SslMode.Require;
    builderConn.TrustServerCertificate = true;
    
    finalConn = builderConn.ToString();
}
else
{
    finalConn = rawConn;
}

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddDbContext<DbConexy>(options =>
    options.UseNpgsql(finalConn));

builder.Services.AddScoped<IConexyRepository, ConexyRepository>();
builder.Services.AddScoped<IConexyService, ConexyService>();

var app = builder.Build();

// 4. Безопасные миграции
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DbConexy>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();
app.MapControllers();
app.Run();
