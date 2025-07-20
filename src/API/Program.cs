using API.Extensions;
using Asp.Versioning;
using Common.Infrastructure;
using Microsoft.OpenApi.Models;
using Modules.Accounts.Infrastructure;
using Modules.Users.Infrastructure;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Assembly[] moduleAssemblies = new[]
{
    typeof(CommonModuleInstaller).Assembly,
    typeof(UsersModuleInstaller).Assembly,
    typeof(AccountsModuleInstaller).Assembly
};

var assemblies = AppDomain.CurrentDomain
    .GetAssemblies()
    .Concat(moduleAssemblies)
    .Distinct()
    .ToList();


var installers = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModuleInstaller).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IModuleInstaller)Activator.CreateInstance(t)!)
            .ToList();

foreach (IModuleInstaller installer in installers)
{
    Console.WriteLine($"Installing module: {installer.GetType().FullName}");
    installer.Install(builder.Services, builder.Configuration);
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
    {
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Enter a valid JSON web token here",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

    });


builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, loggerConfigruration) => loggerConfigruration.ReadFrom.Configuration(builder.Configuration)
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = "http://seq-logging:5341/ingest/otlp/v1/logs";
        options.Protocol = OtlpProtocol.HttpProtobuf;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "API",
            ["deployment.environment"] = "Development"
        };
    }), true, writeToProviders: false);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("API"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter());


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc() // This is needed for controllers
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
    app.ApplyMigrations();
}

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
