namespace SocialMediaProfileScraperDemo.Scraper.Browser;

public interface IScraperBrowserAuthenticator
{
    Task AuthIfNeededAsync();
    void IncrementProfilePageLoadCountForSession();
}