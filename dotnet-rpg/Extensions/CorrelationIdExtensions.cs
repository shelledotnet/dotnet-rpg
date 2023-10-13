using dotnet_rpg.Middlewear;

namespace dotnet_rpg.Extensions
{
    public static class CorrelationIdExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogHeaderMiddleware>();
        }
    }
}
