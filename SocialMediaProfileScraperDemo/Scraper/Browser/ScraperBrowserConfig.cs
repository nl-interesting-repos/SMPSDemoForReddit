using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace SocialMediaProfileScraperDemo.Scraper.Browser;

public class ScraperBrowserConfig
{
    public static ChromeOptions GetChromeOptions(IConfiguration config)
    {
        var chromeOptions = new ChromeOptions();
        
        var arguments = new List<string>
        {
            "--disable-webgl",
            "--disable-notifications",
            "--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36",
            "--log-level=OFF"
        };
        
        if (config.GetValue<bool>("BrowserSettings:RunHeadless"))
        {
            arguments.Add("headless");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            arguments.Add("--no-sandbox");
            arguments.Add("--disable-dev-shm-usage");
        }

        chromeOptions.AddArguments(arguments);
        
        if (config.GetValue<bool>("BrowserSettings:LoadImages") == false)
        {
            chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);
        }
        
        return chromeOptions;
    }
    public static FirefoxOptions GetFirefoxOptions(IConfiguration config)
    {
        var firefoxOptions = new FirefoxOptions();

        firefoxOptions.LogLevel = FirefoxDriverLogLevel.Error;
        
        if (config.GetValue<bool>("BrowserSettings:LoadImages") == false)
        {
            firefoxOptions.SetPreference("permissions.default.image", 2);
        }
        
        firefoxOptions.SetPreference("general.useragent.override", "Mozilla/5.0 (X11; Linux x86_64; rv:101.0) Gecko/20100101 Firefox/101.0");
        firefoxOptions.SetPreference("dom.webnotifications.enabled", false);
        firefoxOptions.SetPreference("browser.privatebrowsing.autostart", true);

        return firefoxOptions;
    }
}