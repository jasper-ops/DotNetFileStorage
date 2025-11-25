namespace Jasper.FileStorage.Core;

public class HybridCloneableStream : ICloneableStream, IDisposable {
    private readonly long _threshold;
    private MemoryStream? _mem;
    private string? _temp;

    public HybridCloneableStream(Stream source, long threshold = 8 * 1024 * 1024) { // 8MB

        _threshold = threshold;

        if (source.CanSeek && source.Length < threshold) {
            source.Seek(0, SeekOrigin.Begin);
            _mem = new MemoryStream((int)source.Length);
            source.CopyTo(_mem);
        } else {
            _temp = Path.GetTempFileName();
            using var fs = File.Create(_temp);
            source.CopyTo(fs);
        }
    }

    public Task<Stream> CloneAsync(CancellationToken cancellationToken = default) {
        if (_mem != null) {
            return Task.FromResult<Stream>(new MemoryStream(_mem.ToArray()));
        }

        return Task.FromResult<Stream>(File.OpenRead(_temp!));
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposing) return;

        if (_temp != null && File.Exists(_temp)) {
            File.Delete(_temp);
            _temp = null;
        }

        if (_mem != null) {
            _mem?.Dispose();
            _mem = null;
        }
    }
}