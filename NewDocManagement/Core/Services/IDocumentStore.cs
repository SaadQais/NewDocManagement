using NewDocManagement.Core.Models;

namespace NewDocManagement.Core.Services
{
    public interface IDocumentStore
    {
        Task<Document> GetAsync(string id, CancellationToken cancellationToken = default);
        Task SaveAsync(Document entity, CancellationToken cancellationToken = default);
    }
}
