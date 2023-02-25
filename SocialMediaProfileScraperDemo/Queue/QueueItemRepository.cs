using System.Text;
using Microsoft.Extensions.Logging;
using SocialMediaProfileScraperDemo.Database;
using SocialMediaProfileScraperDemo.MessageQueue;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemRepository : BaseDao
{
    private readonly ILogger<QueueItemRepository> _logger;
    private readonly MessageQueueWrapper _mqWrapper;
    private readonly QueueItemDataDao _dataDao;

    public QueueItemRepository(
        ILogger<QueueItemRepository> logger,
        IDatabaseProvider databaseProvider,
        MessageQueueWrapper mqWrapper,
        QueueItemDataDao dataDao) : base(databaseProvider)
    {
        _logger = logger;
        _mqWrapper = mqWrapper;
        _dataDao = dataDao;
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

    public async Task StoreQueueItemDataAsync(QueueItemData data)
    {
        await _dataDao.StoreAsync(data);
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
}