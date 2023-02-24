namespace SocialMediaProfileScraperDemo.Scraper.Browser;

public static class ScraperBrowserXPathSelectors
{
    public const string IgProfileDisplayName = "//*[contains(@class, '_aa_c')]//span";
    public const string IgProfilePicture = "//img[@class='_aadp']";
    public const string IgProfilePictureAlt = "//img[contains(@alt,'profile picture')]";
    public const string IgProfileBio = "//h1[contains(@class, '_aacl')]";
    public const string IgProfilePostCount = "(//span[@class='_ac2a'])[1]";
    public const string IgProfileFollowerCount = "(//span[@class='_ac2a'])[2]";
    public const string IgProfileFollowingCount = "(//span[@class='_ac2a'])[3]//span";
}