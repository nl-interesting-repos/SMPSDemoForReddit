using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SocialMediaProfileScraperDemo.Scraper.Browser;

public class ScraperBrowser : IScraperBrowser
{
    private readonly ILogger<ScraperBrowser> _logger;
    private readonly IWebDriver _webDriver;
    private readonly Dictionary<string, IScraperBrowserAuthenticator> _authenticators;

    public ScraperBrowser(
        ILogger<ScraperBrowser> logger, 
        IWebDriver webDriver, 
        Dictionary<string, IScraperBrowserAuthenticator> authenticators)
    {
        _logger = logger;
        _webDriver = webDriver;
        _authenticators = authenticators;
    }

    public void NavigateToUrl(
        string url,
        bool incrementPageLoadCount = false)
    {
        _logger.LogTrace($"Navigating to '{url}'");
        
        _webDriver.Navigate().GoToUrl(url);
        WaitForPage();

        if (incrementPageLoadCount && _authenticators.TryGetValue(Host, out var authenticator))
        {
            authenticator.IncrementProfilePageLoadCountForSession();
        }
    }
    
    public void WaitForPage()
    {
        IWait<IWebDriver> wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5.00));
            
        wait.PollingInterval = TimeSpan.FromMilliseconds(200);
        wait.Until(_ => ((IJavaScriptExecutor)_webDriver).ExecuteScript("return document.readyState").Equals("complete"));
    }

    public Screenshot GetScreenshot()
    {
        return ((ITakesScreenshot)_webDriver).GetScreenshot();
    }

    public IWebElement? GetDomElement(By by)
    {
        try
        {
            return _webDriver.FindElement(by);
        }
        catch (NoSuchElementException)
        {
            return null;
        }
    }
    
    public IWebElement? GetDomElementByXPath(string xpathSelector)
    {
        return GetDomElement(By.XPath(xpathSelector));
    }
    
    public void WaitForElementToAppear(By by, TimeSpan timeOut)
    {
        var sw = Stopwatch.StartNew();
        
        while (GetDomElement(by) == null && sw.Elapsed < timeOut)
        {
            Thread.Sleep(200);
        }
    }

    public string ExecuteScript(string script)
    {
        return (string)((IJavaScriptExecutor)_webDriver).ExecuteScript(script);
    }
    
    public IScraperBrowserAuthenticator GetAuthenticatorForHost(string host)
    {
        if (!_authenticators.ContainsKey(host))
        {
            throw new Exception($"Failed to resolve browser authenticator for host '{host}', check and try again.");
        }
        
        return _authenticators[host];
    }

    public string Host => new Uri(_webDriver.Url).Host.Replace("www.", "");
    public string Url => _webDriver.Url;
    public string PageSource => _webDriver.PageSource;

    public void WaitForSourceCodePhraseToDisappear(string phrase, TimeSpan timeOut)
    {
        var sw = Stopwatch.StartNew();

        while (PageSource.Contains(phrase) && sw.Elapsed < timeOut)
        {
            Thread.Sleep(200);
        }
    }
}