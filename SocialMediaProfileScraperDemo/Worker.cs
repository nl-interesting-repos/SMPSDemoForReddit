using Microsoft.Extensions.Logging;
using SocialMediaProfileScraperDemo.MessageQueue;
using SocialMediaProfileScraperDemo.Queue;

namespace SocialMediaProfileScraperDemo;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly QueueItemRepository _repository;
    private readonly QueueItemDataLoader _dataLoader;
    private readonly QueueItemDao _queueItemDao;

    public Worker(
        ILogger<Worker> logger, 
        QueueItemRepository repository, 
        QueueItemDataLoader dataLoader, 
        QueueItemDao queueItemDao)
    {
        _logger = logger;
        _repository = repository;
        _dataLoader = dataLoader;
        _queueItemDao = queueItemDao;
    }
    
    public async Task StartAsync()
    {
        while (true)
        {
            var item = _repository.GetNextItem();

            try
            {
                var result = await _dataLoader.LoadDataForItem(item);

                if (result.Data != null)
                {
                    _logger.LogInformation("Storing the data we collected...");
                    await _repository.StoreQueueItemDataAsync(result.Data);
                }
                
                await _queueItemDao.UpdateStatusAsync(item, QueueItemStatus.Checking, $"Loaded with code {result.Code}");
                
                _repository.PublishItemToQueue(MessageQueueNames.CheckerQueue, item);
                _repository.AcknowledgeQueueItem(item.DeliveryTag);
            }
            catch (Exception e)
            {
                await _queueItemDao.UpdateStatusAsync(item, QueueItemStatus.Failed, e.Message);
            }
        }
    }
}