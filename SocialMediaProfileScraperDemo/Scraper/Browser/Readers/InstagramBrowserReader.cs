using OpenQA.Selenium;

namespace SocialMediaProfileScraperDemo.Scraper.Browser.Readers;

public class InstagramBrowserReader : IScraperBrowserReader
{
    private readonly ScraperBrowser _browser;

    public InstagramBrowserReader(ScraperBrowser browser)
    {
        _browser = browser;
    }

    public string GetDisplayNameForProfile()
    {
        return _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfileDisplayName)
            ?.GetAttribute("textContent") ?? "";
    }

    public string GetUsernameForProfile()
    {
        return _browser.Url.TrimEnd('/').Split("/").Last();
    }

    public string GetPictureForProfile()
    {
        var picture = _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfilePicture)
            ?.GetAttribute("src") ?? GetPictureForProfileAlt();
        
        return !string.IsNullOrEmpty(picture) ? picture : GetPictureForProfileAlt();
    }

    private string GetPictureForProfileAlt()
    {
        return _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfilePictureAlt)?
            .GetAttribute("src") ?? "";
    }

    public string GetBiographyForProfile()
    {
        var bioElement = _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfileBio);
        return bioElement == null ? "" : bioElement.GetAttribute("innerText");
    }

    public bool IsProfilePrivate()
    {
        return _browser.PageSource.ToLower().Contains("account is private");
    }

    private int GetNumericCountFromElementAttribute(string xpathSelector, string attribute = "innerText")
    {
        var stringCount = _browser.GetDomElementByXPath(xpathSelector)?
            .GetAttribute(attribute)
            .Replace(",", "");

        return !int.TryParse(stringCount, out var intCount) ? 0 : intCount;
    }

    public Dictionary<string, object> GetOtherDataForProfile()
    {
        var postCount = GetNumericCountFromElementAttribute(ScraperBrowserXPathSelectors.IgProfilePostCount);
        var followerCount = GetNumericCountFromElementAttribute(ScraperBrowserXPathSelectors.IgProfileFollowerCount, "title");
        var followingCount = GetNumericCountFromElementAttribute(ScraperBrowserXPathSelectors.IgProfileFollowingCount);

        return new Dictionary<string, object>
        {
            { "PostCount", postCount },
            { "FollowerCount", followerCount },
            { "FollowingCount", followingCount },
        };
    }

    public void WaitForProfileLoad()
    {
        _browser.WaitForElementToAppear(By.XPath(ScraperBrowserXPathSelectors.IgProfileDisplayName), TimeSpan.FromSeconds(5));
    }
}