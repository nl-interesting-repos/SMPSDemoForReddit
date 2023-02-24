using OpenQA.Selenium;

namespace SocialMediaProfileScraperDemo.Scraper.Browser.Readers;

public class TikTokBrowserReader : IScraperBrowserReader
{
    private readonly IWebDriver _webDriver;

    public TikTokBrowserReader(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public string GetDisplayNameForProfile()
    {
        throw new NotImplementedException();
    }

    public string GetUsernameForProfile()
    {
        throw new NotImplementedException();
    }

    public string GetPictureForProfile()
    {
        throw new NotImplementedException();
    }

    public string GetBiographyForProfile()
    {
        throw new NotImplementedException();
    }

    public bool IsProfilePrivate()
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, object> GetOtherDataForProfile()
    {
        throw new NotImplementedException();
    }

    public void WaitForProfileLoad()
    {
        throw new NotImplementedException();
    }
}