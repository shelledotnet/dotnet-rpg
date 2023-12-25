global using dotnet_rpg.domain.Models;
using dotnet_rpg.domain.Data;
using dotnet_rpg.domain.Services;
using dotnet_rpg.Extensions;
using dotnet_rpg.Middlewear;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;


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

    builder.Services.AddControllers(configure =>
    {
        configure.ReturnHttpNotAcceptable = true;
    }).AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddXmlDataContractSerializerFormatters()//for xml output
        .ConfigureApiBehaviorOptions(options =>
        {

            options.SuppressModelStateInvalidFilter = true;
        });//for custom manipulate of message


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSwaggerApiKeySecurity();
        c.AddSwaggerApiKeyAuthorization();
    });

    var projectOptions = builder.Configuration.GetSection(nameof(ProjectOptions)).Get<ProjectOptions>();
    var tokenvalidationParameter = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = projectOptions.Issuer,
        ValidAudience = projectOptions.Audience,
        RequireExpirationTime = true,  //wants the token to expire we can make it false at development time
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(projectOptions.SecreteKey))
        //ClockSkew = TimeSpan.Zero
    };
    builder.Services.AddSingleton(tokenvalidationParameter);//i register this in ioc-container to be able to re-use this anywhere

    //registeering authentiction service to use JwtBearerDefaults
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenvalidationParameter;  //i can pick this any were


            });

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

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
         builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
    });
    builder.Services.AddDbContextFactory<EmployeeManagerDbContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("IMSConnection")));

    builder.Services.AddTransient<IPropertyMappingService,PropertyMappingService>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();




    var app = builder.Build();

#if DEBUG
    #region This will create the Db and run all pending migrations if not exist when application start***this should be discourage at production
    await EnsureDatabaseIsMigrated(app.Services);
    async Task EnsureDatabaseIsMigrated(IServiceProvider services)
    {
        using var scope = services.CreateScope();//the scope help us to get the EmployeeManagerDbContext that we have injected
        using var ctx = scope.ServiceProvider.GetService<EmployeeManagerDbContext>(); 
        if (ctx is not null)
        {
            await ctx.Database.MigrateAsync();//this method MigrateAsync(); will help create DB if not exist and likewise run all pending migration
        }
    }
    #endregion
#else
#endif


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An Unexpected fault happened. Try again later.");
            });
        });
    }
    app.UseCorrelationId();
    app.UseHttpsRedirection();
    app.UseCors();//add this after UserRouting and before UseEndpoints  or UseAuthorization();
    app.UseRequesResponse();
    app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
    app.UseAuthentication();
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