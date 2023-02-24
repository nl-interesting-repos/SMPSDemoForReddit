namespace SocialMediaProfileScraperDemo.Scraper.Browser;

public interface IScraperBrowserReader
{
    string GetDisplayNameForProfile();
    string GetUsernameForProfile();
    string GetPictureForProfile();
    string GetBiographyForProfile();
    bool IsProfilePrivate();
    Dictionary<string, object> GetOtherDataForProfile();
    void WaitForProfileLoad();
}