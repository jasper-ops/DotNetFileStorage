using Jasper.FileStorage.Core;
using Jasper.FileStorage.Core.Abstracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class FileStorageAspNetCore {
    public static IServiceCollection AddFileStorage(this IServiceCollection services, Action<CompositeStorageBuilder> config) {
        var builder = new CompositeStorageBuilder();

        config(builder);

        var provider = builder.Build();

        services.AddSingleton<IFileStorageProvider>(provider);

        return services;
    }
}