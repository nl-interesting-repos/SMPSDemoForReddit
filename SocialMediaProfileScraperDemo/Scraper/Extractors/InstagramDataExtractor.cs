using SocialMediaProfileScraperDemo.Scraper.Browser;

namespace SocialMediaProfileScraperDemo.Scraper.Extractors;

public class InstagramDataExtractor : IScraperDataExtractor
{
    private readonly IScraperBrowser _browser;

    public InstagramDataExtractor(IScraperBrowser browser)
    {
        _browser = browser;
    }

    public string GetDisplayName()
    {
        return _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfileDisplayName)
            ?.GetAttribute("textContent") ?? "";
    }

    public string GetUsername()
    {
        return _browser.Url.TrimEnd('/').Split("/").Last();
    }

    public string GetDisplayPicture()
    {
        var picture = _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfilePicture)
            ?.GetAttribute("src") ?? GetDisplayPictureAlternative();

        return !string.IsNullOrEmpty(picture) ? picture : GetDisplayPictureAlternative();
    }

    public string GetBiography()
    {
        var bioElement = _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfileBio);
        return bioElement == null ? "" : bioElement.GetAttribute("innerText");
    }

    public bool IsPrivate()
    {
        return _browser.PageSource.ToLower().Contains("account is private");
    }

    public Dictionary<string, object> GetOtherData()
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

    public byte[] GetScreenshotBytes()
    {
        return _browser.GetScreenshot().AsByteArray;
    }

    private int GetNumericCountFromElementAttribute(string xpathSelector, string attribute = "innerText")
    {
        var stringCount = _browser.GetDomElementByXPath(xpathSelector)?
            .GetAttribute(attribute)
            .Replace(",", "");

        return !int.TryParse(stringCount, out var intCount) ? 0 : intCount;
    }

    private string GetDisplayPictureAlternative()
    {
        return _browser.GetDomElementByXPath(ScraperBrowserXPathSelectors.IgProfilePictureAlt)?
            .GetAttribute("src") ?? "";
    }
}