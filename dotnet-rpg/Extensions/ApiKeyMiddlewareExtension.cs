using dotnet_rpg.AttributeUsed;
using System.Runtime.CompilerServices;

namespace dotnet_rpg.Extensions
{
    public static class ApiKeyMiddlewareExtension
    {
        public static IApplicationBuilder UseApiKey(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAttribute>();
        }
    }
}
