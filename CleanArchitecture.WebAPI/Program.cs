using CleanArchitecture.Application;
using CleanArchitecture.Persistence;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.WebAPI.Extensions;
using CleanArchitecture.WebAPI.MiddleWare;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using NLog.Web;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
// Configure NLog for logging
builder.Logging.ClearProviders();
builder.Logging.AddNLog();
NLogBuilder.ConfigureNLog("nlog.config");
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure other services
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureApplication();
builder.Services.ConfigureApiBehavior();
builder.Services.ConfigureCorsPolicy();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddNLog();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.UseAuthentication();
app.UseMiddleware<RequestLogMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
//Creating DataContext within the scope
//using (var serviceScope = app.Services.CreateScope())
//{
//    var services = serviceScope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<DataContext>();
//        // This will automatically apply any pending migrations
//        context.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while migrating the database.");
//    }
//}
// NLog Logic
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = NLog.LogManager.GetCurrentClassLogger();
        logger.Error(ex, "An error occurred while migrating the database.");
    }
}
app.Run();


