using SocialMediaProfileScraperDemo.Database;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemDao : BaseDao
{
    public QueueItemDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
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