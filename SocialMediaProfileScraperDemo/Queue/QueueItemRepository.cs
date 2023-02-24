using System.Text;
using Microsoft.Extensions.Logging;
using SocialMediaProfileScraperDemo.MessageQueue;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemRepository
{
    private readonly ILogger<QueueItemRepository> _logger;
    private readonly MessageQueueWrapper _mqWrapper;
    private readonly QueueItemDataDao _dataDao;

    public QueueItemRepository(ILogger<QueueItemRepository> logger, MessageQueueWrapper mqWrapper, QueueItemDataDao dataDao)
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
}