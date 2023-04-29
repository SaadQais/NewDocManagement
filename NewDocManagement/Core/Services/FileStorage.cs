using Microsoft.Extensions.Options;
using NewDocManagement.Core.Options;
using Storage.Net.Blobs;

namespace NewDocManagement.Core.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly IBlobStorage _blobStorage;

        public FileStorage(IOptions<DocumentStorageOptions> options) => _blobStorage = options.Value.BlobStorageFactory();

        public Task WriteAsync(Stream data, string fileName, CancellationToken cancellationToken = default) =>
            _blobStorage.WriteAsync(fileName, data, false, cancellationToken);

        public Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default) =>
            _blobStorage.OpenReadAsync(fileName, cancellationToken);
    }
}
