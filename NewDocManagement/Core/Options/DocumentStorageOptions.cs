using Storage.Net;
using Storage.Net.Blobs;

namespace NewDocManagement.Core.Options
{
    public class DocumentStorageOptions
    {
        public Func<IBlobStorage> BlobStorageFactory { get; set; } = StorageFactory.Blobs.InMemory;
    }
}
