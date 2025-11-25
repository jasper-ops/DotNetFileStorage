using HeyRed.Mime;
using Jasper.FileStorage.Core.Abstracts;
using TOS;
using TOS.Model;

namespace Jasper.FileStorage.VolcengineOss;

public class VolcengineOssStorage(ITosClient client, string bucketName) : IFileStorageProvider {
    private static string GetContentTypeByFileName(string fileName) {
        return MimeTypesMap.GetMimeType(fileName);
    }

    public Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default) {
        return Task.Run(() => {
            var result = client.GetObject(new GetObjectInput {
                Bucket = bucketName,
                Key = path
            });

            return result.Content;
        }, cancellationToken);
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default) {
        return Task.Run(() => {
            client.DeleteObject(new DeleteObjectInput() {
                Bucket = bucketName,
                Key = path
            });
        }, cancellationToken);
    }

    public Task SaveAsync(string path, Stream fileStream, CancellationToken cancellationToken = default) {
        return Task.Run(() => {
            client.PutObject(new PutObjectInput {
                Bucket = bucketName,
                Key = path,
                Content = fileStream,
                ContentType = GetContentTypeByFileName(path)
            });
        }, cancellationToken);
    }

    public Task CopyToAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default) {
        return Task.Run(() => {
            client.CopyObject(new CopyObjectInput {
                SrcBucket = bucketName,
                SrcKey = sourcePath,
                Bucket = bucketName,
                Key = destinationPath
            });
        }, cancellationToken);
    }

    public async Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default) {
        await CopyToAsync(sourcePath, destinationPath, cancellationToken);
        await DeleteAsync(sourcePath, cancellationToken);
    }
}