namespace NewDocManagement.Core.Services
{
    public interface IFileStorage
    {
        Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default);
        Task WriteAsync(Stream data, string fileName, CancellationToken cancellationToken = default);
    }
}
