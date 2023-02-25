using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SocialMediaProfileScraperDemo;

internal static class Program
{
    private static async Task Main()
    {
        var host = Startup.CreateHost();
        var services = host.Services;

        var config = services.GetRequiredService<IConfiguration>();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        var worker = services.GetRequiredService<Worker>();
        await worker.StartAsync();
    }
}