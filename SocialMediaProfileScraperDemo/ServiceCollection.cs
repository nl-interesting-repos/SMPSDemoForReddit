using System.Reflection;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using RabbitMQ.Client;
using SocialMediaProfileScraperDemo.Database;
using SocialMediaProfileScraperDemo.MessageQueue;
using SocialMediaProfileScraperDemo.Queue;
using SocialMediaProfileScraperDemo.Scraper;
using SocialMediaProfileScraperDemo.Scraper.Accounts;
using SocialMediaProfileScraperDemo.Scraper.Browser;
using SocialMediaProfileScraperDemo.Scraper.Browser.Authenticators;
using SocialMediaProfileScraperDemo.Scraper.Extractors;
using SocialMediaProfileScraperDemo.Scraper.Presenters;
using SocialMediaProfileScraperDemo.Storage;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SocialMediaProfileScraperDemo;

public static class ServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var delaySettings = new DelaySettings();
        config.GetSection("DelaySettings").Bind(delaySettings);
        
        serviceCollection.AddSingleton(delaySettings);

        DatabaseServiceCollection.AddServices(serviceCollection, config);

        serviceCollection.AddSingleton<IConnectionFactory, ConnectionFactory>(provider => new ConnectionFactory
        {
            Uri = new Uri(config.GetConnectionString("RabbitMq"))
        });
        
        serviceCollection.AddSingleton<MessageQueueWrapper>();
        serviceCollection.AddSingleton<QueueItemRepository>();
        serviceCollection.AddSingleton<QueueItemDataDao>();
        serviceCollection.AddSingleton<Worker>();
        
        serviceCollection.AddSingleton<ScraperAccountDao>();
        serviceCollection.AddSingleton<ScraperAccountRepository>();
        
        AddStorageServices(serviceCollection, config);

        serviceCollection.AddSingleton(provider => new Dictionary<string, IScraperDataPresenter>
        {
            { "facebook.com", provider.GetRequiredService<FacebookDataPresenter>() },
            { "instagram.com", provider.GetRequiredService<InstagramDataPresenter>() },
            { "tiktok.com", provider.GetRequiredService<TikTokDataPresenter>() }
        });

        serviceCollection.AddSingleton(provider => new Dictionary<string, IScraperDataExtractor>
        {
            { "facebook.com", provider.GetRequiredService<FacebookDataExtractor>() },
            { "instagram.com", provider.GetRequiredService<InstagramDataExtractor>() },
            { "tiktok.com", provider.GetRequiredService<TikTokDataExtractor>() }
        });

        serviceCollection.AddSingleton(provider => new Dictionary<string, IScraperBrowserAuthenticator>
        {
            { "facebook.com", provider.GetRequiredService<FacebookBrowserAuthenticator>() },
            { "instagram.com", provider.GetRequiredService<InstagramBrowserAuthenticator>() },
            { "tiktok.com", provider.GetRequiredService<TikTokBrowserAuthenticator>() },
            { "tinder.com", provider.GetRequiredService<TinderBrowserAuthenticator>() },
        });

        serviceCollection.AddSingleton<ScraperLoader>();

        serviceCollection.AddSingleton<FacebookDataExtractor>();
        serviceCollection.AddSingleton<FacebookBrowserAuthenticator>();
        serviceCollection.AddSingleton<InstagramDataExtractor>();
        serviceCollection.AddSingleton<InstagramBrowserAuthenticator>();
        serviceCollection.AddSingleton<TikTokDataExtractor>();
        serviceCollection.AddSingleton<TikTokBrowserAuthenticator>();
        serviceCollection.AddSingleton<TinderBrowserAuthenticator>();
        
        AddSeleniumServices(serviceCollection, config);
    }
    
    private static void AddStorageServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var accessKey = config["Spaces:AccessKey"];
        var secretKey = config["Spaces:SecretKey"];
        var cdnUrl = config["Spaces:CdnUrl"];
        var bucketName = config["Spaces:BucketName"];
        
        serviceCollection.AddSingleton(_ => new AmazonS3Client(accessKey, secretKey, new AmazonS3Config
        {
            ServiceURL = config["Spaces:ServiceUrl"]
        }));
        
        serviceCollection.AddSingleton<IStorageClient>(provider => new SpacesStorageClient(provider.GetRequiredService<AmazonS3Client>(), bucketName, cdnUrl));
    }
    
    private static void AddSeleniumServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<ScraperBrowser>();

        var driver = config["BrowserSettings:Driver"];

        switch (driver)
        {
            case "chrome":
            {
                var chromeOptions = ScraperBrowserConfig.GetChromeOptions(config);
                serviceCollection.AddSingleton<IWebDriver>(_ => new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions));
                break;
            }
            case "firefox":
            {
                var options = ScraperBrowserConfig.GetFirefoxOptions(config);
                serviceCollection.AddSingleton<IWebDriver>(_ => new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options));
                break;
            }
            default:
                throw new Exception($"Unknown browser driver '{driver}' check and try again.");
        }
    }
}