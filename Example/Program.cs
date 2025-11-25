using Jasper.FileStorage.Core;
using Jasper.FileStorage.Core.Abstracts;
using Jasper.FileStorage.Core.Options;
using Jasper.FileStorage.VolcengineOss;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example;

class Program {
    async static Task Main(string[] args) {

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .Build();

        // Can only be a relative path
        const string path = "2025/11/11/a.txt";

        var services = new ServiceCollection();

        services.AddFileStorage(builder => {
            builder
                .UseLocalFileStorageProvider(new LocalFileStorageOptions {
                    BasePath = @"d:\"
                })
                .UseVolcengineOssStorage(new UseVolcengineOssStorageOptions {
                    AccessKey = config["Volcengine:AccessKey"]!,
                    SecretKey = config["Volcengine:SecretKey"]!,
                    Bucket = config["Volcengine:BucketName"]!
                });
        });
        var sp = services.BuildServiceProvider();

        var fileStorage = sp.GetRequiredService<IFileStorageProvider>();


        // Write
        var text = "Hello World"u8.ToArray();
        var ms = new MemoryStream();
        ms.Write(text);
        await fileStorage.SaveAsync(path, ms);


        // Read(untile read by a Provider in its mounted order, or until a `FileNotFoundException` is thrown)
        using var reader = new StreamReader(await fileStorage.GetAsync(path));
        var content = await reader.ReadToEndAsync();
        
        Console.WriteLine("读取到的内容： " + content);
    }
}