using CleanArchitecture.WebAPI.MiddleWare;

namespace CleanArchitecture.WebAPI.Extensions
{
    public static class RequestLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
