namespace Jasper.FileStorage.Core;

public interface ICloneableStream {
    Task<Stream> CloneAsync(CancellationToken cancellationToken = default);
}