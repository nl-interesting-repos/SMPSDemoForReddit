using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SocialMediaProfileScraperDemo.Database;
using SocialMediaProfileScraperDemo.MessageQueue;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemRepository : BaseDao
{
    private readonly ILogger<QueueItemRepository> _logger;
    private readonly MessageQueueWrapper _mqWrapper;

    public QueueItemRepository(
        ILogger<QueueItemRepository> logger,
        IDatabaseProvider databaseProvider,
        MessageQueueWrapper mqWrapper) : base(databaseProvider)
    {
        _logger = logger;
        _mqWrapper = mqWrapper;
    }

    public QueueItem GetNextItem()
    {
        _logger.LogInformation("Fetching an item from the queue...");
        
        while (true)
        {
            var item = _mqWrapper.BasicGet(MessageQueueNames.LoaderQueue);

            if (item == null)
            {
                continue;
            }

            var message = Encoding.Default.GetString(item.Body.ToArray());
            var parts = message.Split(",");
            var queueItemId = int.Parse(parts[0]);

            return new QueueItem(queueItemId, parts[1], (QueueItemStatus)int.Parse(parts[2]), item.DeliveryTag);
        }
    }

    public void PublishItemToQueue(string queueName, QueueItem item)
    {
        _mqWrapper.BasicPublish(queueName, $"{item.Id},{item.Url}");
    }

    public void AcknowledgeQueueItem(ulong deliveryTag)
    {
        _mqWrapper.BasicAck(deliveryTag);
    }

    public async Task UpdateStatusAsync(QueueItem item, QueueItemStatus statusId, string reason)
    {
        await QueryAsync("UPDATE scraper_queue_items SET status_id = @statusId WHERE id = @id;", new Dictionary<string, object>
        {
            { "statusId", statusId },
            { "id", item.Id }
        });

        await QueryAsync("INSERT INTO scraper_queue_item_status_changes (queue_item_id, user_id, from_status_id, to_status_id, reason, changed_at) VALUES (@queueItemId, @userId, @fromId, @toId, @reason, NOW());", new Dictionary<string, object>
        {
            { "queueItemId", item.Id },
            { "userId", 1 },
            { "fromId", item.Status },
            { "toId", statusId },
            { "reason", reason }
        });

        item.Status = statusId;
    }

    public async Task StoreDataAsync(QueueItemData queueItemData)
    {
        await QueryAsync("INSERT INTO `scraper_queue_item_data` (queue_item_id, screenshot_url, display_name, username, picture, biography, is_private, other_data, loaded_at) VALUES (@itemId, @screenshot, @name, @username, @picture, @bio, @isPrivate, @otherData, @loadedAt);", new Dictionary<string, object>()
        {
            { "itemId", queueItemData.ItemId },
            { "screenshot", queueItemData.ScreenshotUrl },
            { "name", queueItemData.DisplayName },
            { "username", queueItemData.Username },
            { "picture", queueItemData.Picture },
            { "bio", queueItemData.Biography },
            { "isPrivate", queueItemData.IsPrivate },
            { "otherData", JsonConvert.SerializeObject(queueItemData.OtherData) },
            { "loadedAt", queueItemData.LoadedAt },
        });
    }
}