namespace Jasper.FileStorage.Core.Abstracts;

public interface IFileStorageProvider {
    Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default);
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    Task SaveAsync(string path, Stream fileStream, CancellationToken cancellationToken = default);
    Task CopyToAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
    Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
}