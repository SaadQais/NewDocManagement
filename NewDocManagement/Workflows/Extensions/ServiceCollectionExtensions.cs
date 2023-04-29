using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Providers.Workflows;
using Elsa.Server.Hangfire.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using NewDocManagement.Workflows.Activities;
using NewDocManagement.Workflows.Handlers;
using NewDocManagement.Workflows.Scripting.JavaScript;
using Storage.Net;

namespace NewDocManagement.Workflows.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
        {
            return services.AddElsa(configureDb);
        }

        private static IServiceCollection AddElsa(this IServiceCollection services, Action<DbContextOptionsBuilder> config)
        {
            services
                .AddElsa(elsa => elsa

                    // Use EF Core's SQLite provider to store workflow instances and bookmarks.
                    .UseEntityFrameworkPersistence(config)

                    // Ue Console activities for testing & demo purposes.
                    .AddConsoleActivities()

                    // Use Hangfire to dispatch workflows from.
                    .UseHangfireDispatchers()

                    // Configure Email activities.
                    .AddEmailActivities()

                    // Configure HTTP activities.
                    .AddHttpActivities()

                    // Add custom activities
                    .AddActivitiesFrom<GetDocument>()
                    .AddActivitiesFrom<ArchiveDocument>()
                    .AddActivitiesFrom<ZipFile>()
                    .AddActivitiesFrom<UpdateBlockchain>()
                );

            // Get directory path to current assembly.
            var currentAssemblyPath = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location);

            // Configure Storage for BlobStorageWorkflowProvider with a directory on disk from where to load workflow definition
            // JSON files from the local "Workflows" folder.
            services.Configure<BlobStorageWorkflowProviderOptions>(options => options.BlobStorageFactory = () => 
                StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath, "Workflows")));

            services.AddNotificationHandlersFrom<StartDocumentWorkflows>();

            // Register custom type definition provider for JS intellisense.
            services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();

            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

            return services;
        }
    }
}
