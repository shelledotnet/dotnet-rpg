global using dotnet_rpg.domain.Models;
using dotnet_rpg.domain.Services;
using dotnet_rpg.Extensions;
using dotnet_rpg.Middlewear;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;


#region configurationBuilder
var configurationBuilder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false, true)
                                .AddJsonFile("appsettings.Development.json", optional: true)
                                .Build();

Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(configurationBuilder)
                 .CreateBootstrapLogger();
//.CreateLogger(); 
#endregion




try
{
    Log.Information("DotNet-RPG  starting up...");
    var builder = WebApplication.CreateBuilder(args);
    #region add this middleweare .UseSerilog() means enforth we are using serilog as our loger replacing the default logger from Dotnet

    #endregion
    builder.Host.UseSerilog((context, configurationBuilder) => configurationBuilder
                                                             .ReadFrom.Configuration(context.Configuration));

    // Add services to the container.

    builder.Services.AddControllers()//for custom response on the model
        .ConfigureApiBehaviorOptions(options =>
        {

            options.SuppressModelStateInvalidFilter = true;
        });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c=>c.AddSwaggerApiKeySecurity());
    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddScoped<ICharacterService, CharacterServices>();
    builder.Services.AddOptions<ProjectOptions>()
                    .BindConfiguration(nameof(ProjectOptions))
                    .ValidateDataAnnotations()
                     .Validate(options =>
                     {
                         if (options.XApiKey != "se") return false;
                         return true;
                     })
                   .ValidateOnStart();
 
    
    var app = builder.Build();
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseCorrelationId();
    app.UseHttpsRedirection();
    app.UseRequesResponse();
    app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.OrdinalIgnoreCase)) throw;
    Log.Fatal(ex, "DotNet-RPG failed to start corretly , Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}