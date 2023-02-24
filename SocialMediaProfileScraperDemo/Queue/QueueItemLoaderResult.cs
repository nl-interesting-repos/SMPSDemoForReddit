namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemLoaderResult
{
    public QueueItemLoaderResult(int code, QueueItemData data)
    {
        Code = code;
        Data = data;
    }

    public int Code { get; }
    public QueueItemData? Data { get; }
}