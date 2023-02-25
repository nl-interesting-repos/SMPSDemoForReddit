using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using SocialMediaProfileScraperDemo.Queue;
using SocialMediaProfileScraperDemo.Scraper.Browser;

namespace SocialMediaProfileScraperDemo.Scraper.Presenters;

public class InstagramDataPresenter : IScraperDataPresenter
{
    private readonly IScraperBrowser _browser;
    private readonly IConfiguration _configuration;

    public InstagramDataPresenter(IScraperBrowser browser, IConfiguration configuration)
    {
        _browser = browser;
        _configuration = configuration;
    }

    public async Task PresentDataForItemAsync(QueueItem item)
    {
        if (_configuration.GetValue<bool>("BrowserSettings:Authentication"))
        {
            var host = new Uri(item.Url).Host.Replace("www.", "");
            var browserAuthenticator = _browser.GetAuthenticatorForHost(host);

            await browserAuthenticator.AuthIfNeededAsync();
        }

        _browser.NavigateToUrl(item.Url, true);
        _browser.WaitForElementToAppear(By.XPath(ScraperBrowserXPathSelectors.IgProfileDisplayName), TimeSpan.FromSeconds(5));
    }
}