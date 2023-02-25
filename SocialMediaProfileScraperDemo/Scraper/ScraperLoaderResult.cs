using SocialMediaProfileScraperDemo.Queue;

namespace SocialMediaProfileScraperDemo.Scraper;

public class ScraperLoaderResult
{
    public ScraperLoaderResult(int code, QueueItemData data)
    {
        Code = code;
        Data = data;
    }

    public int Code { get; }
    public QueueItemData? Data { get; }
}