using Microsoft.EntityFrameworkCore;
using NewDocManagement.Core.Services;
using NewDocManagement.Persistence.HostedServices;
using NewDocManagement.Persistence.Services;

namespace NewDocManagement.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainPersistence(this IServiceCollection services, string connectionString)
        {
            var migrationsAssemblyName = typeof(SqliteDocumentDbContextFactory).Assembly.GetName().Name;

            return services
                .AddPooledDbContextFactory<DocumentDbContext>(x => x.UseSqlite(connectionString, db => db.MigrationsAssembly(migrationsAssemblyName)))
                .AddSingleton<IDocumentStore, EFCoreDocumentStore>()
                .AddSingleton<IDocumentTypeStore, EFCoreDocumentTypeStore>()
                .AddHostedService<RunMigrations>();
        }
    }
}
