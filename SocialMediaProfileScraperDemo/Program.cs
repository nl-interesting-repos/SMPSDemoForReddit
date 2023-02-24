using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SocialMediaProfileScraperDemo.Scraper.Browser;
using SocialMediaProfileScraperDemo.Scraper.Browser.Authenticators;
using SocialMediaProfileScraperDemo.Scraper.Browser.Readers;

namespace SocialMediaProfileScraperDemo;

internal static class Program
{
    private static async Task Main()
    {
        var host = Startup.CreateHost();
        var services = host.Services;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(services.GetRequiredService<IConfiguration>())
            .CreateLogger();

        var browser = services.GetRequiredService<ScraperBrowser>();

        browser.RegisterServicesForHost(
            "facebook.com",
            services.GetRequiredService<FacebookBrowserReader>(),
            services.GetRequiredService<FacebookBrowserAuthenticator>());

        browser.RegisterServicesForHost(
            "instagram.com",
            services.GetRequiredService<InstagramBrowserReader>(),
            services.GetRequiredService<InstagramBrowserAuthenticator>());

        browser.RegisterServicesForHost(
            "tiktok.com",
            services.GetRequiredService<TikTokBrowserReader>(),
            services.GetRequiredService<TikTokBrowserAuthenticator>());

        var worker = services.GetRequiredService<Worker>();
        
        await worker.StartAsync();
    }
}