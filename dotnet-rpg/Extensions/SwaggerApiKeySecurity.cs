using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace dotnet_rpg.Extensions
{
    public static class SwaggerApiKeySecurity
    {
        public static void AddSwaggerApiKeySecurity(this SwaggerGenOptions c)
        {
            #region BasicAuth
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "Apikey must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = "XApiKey",
                In = ParameterLocation.Header,
                Scheme = "ApiKeywScheme"
            });

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };
            var requirement = new OpenApiSecurityRequirement
                {
                   { key, new List<string>() }
                };

            c.AddSecurityRequirement(requirement);
            #endregion

        }
    }
}
