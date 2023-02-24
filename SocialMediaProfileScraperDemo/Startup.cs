using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SocialMediaProfileScraperDemo;

public static class Startup
{
    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, collection) => ServiceCollection.AddServices(collection, context.Configuration))
            .UseSerilog()
            .Build();
    }
}