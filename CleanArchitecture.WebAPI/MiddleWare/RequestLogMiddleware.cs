using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Persistence.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
namespace CleanArchitecture.WebAPI.MiddleWare
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public RequestLogMiddleware(
            RequestDelegate next,
            ILogger<RequestLogMiddleware> logger,
            IServiceScopeFactory scopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Request received: {Method} {Path}", context.Request.Method, context.Request.Path);
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await LogExceptionAsync(dbContext, ex);
                }
            }
            _logger.LogInformation("Response sent: {StatusCode}", context.Response.StatusCode);
        }
        //private async Task LogExceptionAsync(DataContext dbContext, Exception exception)
        //{
        //    var logEntry = new Log
        //    {
        //        LogLevel = "Error",
        //        Timestamp = DateTime.UtcNow,
        //        Message = exception.Message,
        //        Exception = exception.ToString()
        //    };
        //    dbContext.Logs.Add(logEntry);
        //    await dbContext.SaveChangesAsync();
        //}
        //NLog COde
        private async Task LogExceptionAsync(DataContext dbContext, Exception exception)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            logger.Error(exception, "An error occurred.");

            var logEntry = new Log
            {
                LogLevel = "Error",
                Timestamp = DateTime.UtcNow,
                Message = exception.Message,
                Exception = exception.ToString()
            };
            dbContext.Logs.Add(logEntry);
            await dbContext.SaveChangesAsync();
        }
    }
}
