using Jasper.FileStorage.Core.Abstracts;
using Jasper.FileStorage.Core.Options;
using Jasper.FileStorage.Core.Providers;

namespace Jasper.FileStorage.Core;

public class CompositeStorageBuilder {
    private readonly List<IFileStorageProvider> _providers = [];

    public CompositeStorageBuilder UseProvider(IFileStorageProvider provider) {
        _providers.Add(provider);
        return this;
    }
    
    public CompositeStorageBuilder UseLocalFileStorageProvider(LocalFileStorageOptions options) {
        _providers.Add(new LocalFileStorageProvider(options));

        return this;
    }

    public CompositeStorageProvider Build() {
        return new CompositeStorageProvider(_providers);
    }
}