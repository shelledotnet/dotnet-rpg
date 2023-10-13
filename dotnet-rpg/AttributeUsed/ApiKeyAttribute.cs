using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace dotnet_rpg.AttributeUsed
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiKeyAttribute : Attribute, IAuthorizationFilter
    {

        private const string _ApiKeyName = "XApiKey";
        private readonly ProjectOptions _optionsMonitor;

        public ApiKeyAttribute(IOptionsMonitor<ProjectOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor.CurrentValue;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var apiKeyPresentInHeader = httpContext.Request.Headers.TryGetValue(_ApiKeyName, out var expectedApiKey);
            if (apiKeyPresentInHeader && _optionsMonitor.XApiKey == expectedApiKey
                || httpContext.Request.Path.StartsWithSegments("/swagger"))
            {
                return;
            }
            ServiceFailedResponse response = new() { Message = "unauthorised", Success = false };
            context.Result = new UnauthorizedObjectResult(response);
        }
    }
}
