namespace SocialMediaProfileScraperDemo.Queue;

public record QueueItem(long Id, string Url, QueueItemStatus Status, ulong DeliveryTag)
{
    public QueueItemStatus Status { get; set; } = Status;
}