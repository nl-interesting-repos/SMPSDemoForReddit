using OpenQA.Selenium;

namespace SocialMediaProfileScraperDemo.Scraper.Browser;

public interface IScraperBrowser
{
    void NavigateToUrl(string url, bool incrementPageLoadCount = false);
    void WaitForPage();
    Screenshot GetScreenshot();
    IWebElement? GetDomElement(By by);
    IWebElement? GetDomElementByXPath(string xpathSelector);
    void WaitForElementToAppear(By by, TimeSpan timeOut);
    string ExecuteScript(string script);
    IScraperBrowserAuthenticator GetAuthenticatorForHost(string host);
    string Host { get; }
    string Url { get; }
    string PageSource { get; }
    void WaitForSourceCodePhraseToDisappear(string phrase, TimeSpan timeOut);
}