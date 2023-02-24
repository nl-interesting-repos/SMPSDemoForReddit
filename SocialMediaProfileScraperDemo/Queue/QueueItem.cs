namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItem
{
    public long Id { get; }
    public string Url { get; }
    public QueueItemStatus Status { get; set; }
    public ulong DeliveryTag { get; }

    public QueueItem(long id, string url, QueueItemStatus status, ulong deliveryTag)
    {
        Id = id;
        Url = url;
        Status = status;
        DeliveryTag = deliveryTag;
    }
}