using NewDocManagement.Core.Options;
using NewDocManagement.Core.Services;
using Storage.Net;

namespace NewDocManagement.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);

            return services
                .AddSingleton<ISystemClock, SystemClock>()
                .AddSingleton<IFileStorage, FileStorage>()
                .AddScoped<IDocumentService, DocumentService>();
        }
    }
}
