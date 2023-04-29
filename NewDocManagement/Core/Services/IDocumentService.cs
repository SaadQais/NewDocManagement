using NewDocManagement.Core.Models;

namespace NewDocManagement.Core.Services
{
    public interface IDocumentService
    {
        Task<Document> SaveDocumentAsync(string fileName, Stream data, string documentTypeId, 
            CancellationToken cancellationToken = default);
    }
}
