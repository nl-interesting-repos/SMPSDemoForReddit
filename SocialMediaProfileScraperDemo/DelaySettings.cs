namespace SocialMediaProfileScraperDemo;

public class DelaySettings
{
    public int SecondsToBlockAccountAfterFetched { get; set; }
    public int SecondsToWaitAfterAccountFetchFailed { get; set; }
    public int SecondsToWaitAfterAccountLoginFailed { get; set; }
}