using Elsa;
using Elsa.Activities.Email.Options;
using Elsa.Activities.Http.Options;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Server.Hangfire.Extensions;
using Hangfire;
using Hangfire.SQLite;
using NewDocManagement.Core.Extensions;
using NewDocManagement.Core.Options;
using NewDocManagement.Persistence.Extensions;
using NewDocManagement.Workflows.Extensions;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Storage.Net;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var env = builder.Environment;

var connectionString = config.GetConnectionString("Sqlite");

// Razor Pages (for UI).
builder.Services.AddRazorPages();

// Hangfire (for background tasks).
AddHangfire(builder.Services, connectionString);

// Elsa (workflows engine).
AddWorkflowServices(builder.Services, connectionString);

// Domain services.
AddDomainServices(builder.Services);

// Persistence.
AddPersistenceServices(builder.Services, connectionString);

// Register all Mediatr event handlers from this assembly.
builder.Services.AddNotificationHandlersFrom<Startup>();

// Allow arbitrary client browser apps to access the API for demo purposes only.
// In a production environment, make sure to allow only origins you trust.
builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .WithExposedHeaders("Content-Disposition")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app
    .UseStaticFiles()
    .UseCors()
    .UseRouting()
    .UseHttpActivities() // Install middleware for triggering HTTP Endpoint activities. 
    .UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapControllers(); // Elsa API Endpoints are implemented as ASP.NET API controllers.
    });

app.Run();

void AddHangfire(IServiceCollection services, string connectionString)
{
    services
        .AddHangfire(config => config
            // Use same SQLite database as Elsa for storing jobs. 
            .UseSQLiteStorage(connectionString)
            .UseSimpleAssemblyNameTypeSerializer()

            // Elsa uses NodaTime primitives, so Hangfire needs to be able to serialize them.
            .UseRecommendedSerializerSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)))
        .AddHangfireServer((sp, options) =>
        {
            // Bind settings from configuration.
            config.GetSection("Hangfire").Bind(options);

            // Configure queues for Elsa workflow dispatchers.
            options.ConfigureForElsaDispatchers(sp);
        });
}

void AddWorkflowServices(IServiceCollection services, string connectionString)
{
    services.AddWorkflowServices(context => context.UseSqlite(connectionString));

    // Configure SMTP.
    services.Configure<SmtpOptions>(options => config.GetSection("Elsa:Smtp").Bind(options));

    // Configure HTTP activities.
    services.Configure<HttpActivityOptions>(options => config.GetSection("Elsa:Server").Bind(options));

    // Elsa API (to allow Elsa Dashboard to connect for checking workflow instances).
    services.AddElsaApiEndpoints();
}

void AddDomainServices(IServiceCollection services)
{
    services.AddDomainServices();

    // Configure Storage for DocumentStorage.
    services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = () =>
        StorageFactory.Blobs.DirectoryFiles(Path.Combine(env.ContentRootPath, "App_Data/Uploads")));
}
void AddPersistenceServices(IServiceCollection services, string dbConnectionString)
{
    services.AddDomainPersistence(dbConnectionString);
}
