using ConexyTask.DbContext;
using ConexyTask.Repository;
using ConexyTask.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true";
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
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IConexyRepository, ConexyRepository>();
builder.Services.AddScoped<IConexyService, ConexyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbConexy>();
    context.Database.Migrate();
}

app.UseCors();
app.MapControllers();
app.Run();
