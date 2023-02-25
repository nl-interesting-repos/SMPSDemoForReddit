using Microsoft.Extensions.Logging;
using SocialMediaProfileScraperDemo.MessageQueue;
using SocialMediaProfileScraperDemo.Queue;
using SocialMediaProfileScraperDemo.Scraper;

namespace SocialMediaProfileScraperDemo;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly ScraperLoader _loader;
    private readonly QueueItemRepository _queueItemRepository;

    public Worker(
        ILogger<Worker> logger,
        ScraperLoader loader,
        QueueItemRepository queueItemRepository)
    {
        _logger = logger;
        _loader = loader;
        _queueItemRepository = queueItemRepository;
    }
    
    public async Task StartAsync()
    {
        while (true)
        {
            var item = _queueItemRepository.GetNextItem();
            await ProcessItemAsync(item);
        }
    }

    private async Task ProcessItemAsync(QueueItem item)
    {
        try
        {
            var result = await _loader.GetResultForQueueItemAsync(item);

            if (result.Data != null)
            {
                _logger.LogInformation("Storing the data we collected...");

                await _queueItemRepository.StoreDataAsync(result.Data);
                await _queueItemRepository.UpdateStatusAsync(item, QueueItemStatus.Checking, $"Loaded with code {result.Code}");

                _queueItemRepository.PublishItemToQueue(MessageQueueNames.CheckerQueue, item);
            }
            else
            {
                await _queueItemRepository.UpdateStatusAsync(item, QueueItemStatus.Failed, $"Loaded with code {result.Code}");
            }

            _queueItemRepository.AcknowledgeQueueItem(item.DeliveryTag);
        }
        catch (Exception e)
        {
            await _queueItemRepository.UpdateStatusAsync(item, QueueItemStatus.Failed, e.Message);
        }
    }
}