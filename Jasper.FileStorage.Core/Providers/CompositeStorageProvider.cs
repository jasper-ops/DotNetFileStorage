using Jasper.FileStorage.Core.Abstracts;

namespace Jasper.FileStorage.Core.Providers;

public class CompositeStorageProvider : IFileStorageProvider {
    private readonly IEnumerable<IFileStorageProvider> _providers;

    internal CompositeStorageProvider(IEnumerable<IFileStorageProvider> providers) {
        _providers = providers;
    }

    public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default) {
        foreach (var item in _providers) {
            try {
                return await item.GetAsync(path, cancellationToken);
            } catch (FileNotFoundException) {
                // ignore
            }
        }

        throw new FileNotFoundException();
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default) {
        List<Exception>? errors = null;

        foreach (var item in _providers) {
            try {
                await item.DeleteAsync(path, cancellationToken);
            } catch (Exception ex) {
                errors ??= [];
                errors.Add(ex);
            }
        }

        if (errors != null) throw new AggregateException(errors);
    }

    public async Task SaveAsync(string path, Stream fileStream, CancellationToken cancellationToken = default) {
        using var cloneable = new HybridCloneableStream(fileStream);

        foreach (var provider in _providers) {
            await using var stream = await cloneable.CloneAsync(cancellationToken);
            await provider.SaveAsync(path, stream, cancellationToken);
        }
    }

    public Task CopyToAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default) {
        var tasks = _providers.Select(item => item.CopyToAsync(sourcePath, destinationPath, cancellationToken)).ToArray();
        return Task.WhenAll(tasks);
    }

    public Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default) {
        var tasks = _providers.Select(item => item.MoveAsync(sourcePath, destinationPath, cancellationToken)).ToArray();
        return Task.WhenAll(tasks);
    }
}