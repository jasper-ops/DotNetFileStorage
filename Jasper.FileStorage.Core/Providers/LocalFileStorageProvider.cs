using Jasper.FileStorage.Core.Abstracts;
using Jasper.FileStorage.Core.Options;

namespace Jasper.FileStorage.Core.Providers;

/// <summary>
/// 本地文件存储提供者
/// </summary>
public class LocalFileStorageProvider(LocalFileStorageOptions options) : IFileStorageProvider {
    private readonly string _root = Path.GetFullPath(options.BasePath);

    private string GetSafeFullPath(string relativePath) {
        var fullPath = Path.GetFullPath(Path.Combine(_root, relativePath));

        return !fullPath.StartsWith(_root, StringComparison.OrdinalIgnoreCase)
            ? throw new InvalidOperationException($"非法路径访问: {relativePath}")
            : fullPath;
    }

    public Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default) {
        var stream = new FileStream(GetSafeFullPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);

        return Task.FromResult<Stream>(stream);
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default) {
        File.Delete(GetSafeFullPath(path));
        return Task.CompletedTask;
    }

    public async Task SaveAsync(string path, Stream fileStream, CancellationToken cancellationToken = default) {
        path = GetSafeFullPath(path);

        var dir = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("文件存储路径不合法");
        Directory.CreateDirectory(dir);

        await using var writer = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true
        );
        await fileStream.CopyToAsync(writer, cancellationToken);
    }

    public Task CopyToAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default) {
        File.Copy(GetSafeFullPath(sourcePath), GetSafeFullPath(destinationPath), true);

        return Task.CompletedTask;
    }

    public Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default) {
        File.Move(GetSafeFullPath(sourcePath), GetSafeFullPath(destinationPath), true);

        return Task.CompletedTask;
    }
}