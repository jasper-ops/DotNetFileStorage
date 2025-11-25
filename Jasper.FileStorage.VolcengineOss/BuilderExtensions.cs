using Jasper.FileStorage.VolcengineOss;
using TOS;

namespace Jasper.FileStorage.Core;

public static class BuilderExtensions {
    public static CompositeStorageBuilder UseVolcengineOssStorage(this CompositeStorageBuilder builder, UseVolcengineOssStorageOptions options) {
        var client = TosClientBuilder.Builder().SetAk(options.AccessKey).SetSk(options.SecretKey).Build();

        builder.UseProvider(new VolcengineOssStorage(client, options.Bucket));

        return builder;
    }
}