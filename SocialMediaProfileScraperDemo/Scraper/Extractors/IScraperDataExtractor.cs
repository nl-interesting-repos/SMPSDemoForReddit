namespace SocialMediaProfileScraperDemo.Scraper.Extractors;

public interface IScraperDataExtractor
{
    string GetDisplayName();
    string GetUsername();
    string GetDisplayPicture();
    string GetBiography();
    bool IsPrivate();
    Dictionary<string, object> GetOtherData();
    byte[] GetScreenshotBytes();
}