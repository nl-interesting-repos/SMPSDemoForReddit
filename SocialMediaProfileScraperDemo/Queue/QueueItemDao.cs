using Dapper;
using SocialMediaProfileScraperDemo.Database;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemDao
{
    private readonly IDatabaseProvider _databaseProvider;

    public QueueItemDao(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
    }

    public async Task UpdateStatusAsync(QueueItem item, QueueItemStatus statusId, string reason)
    {
        await using var connection = _databaseProvider.GetConnection();

        await connection.ExecuteAsync("UPDATE scraper_queue_items SET status_id = @statusId WHERE id = @id;", new Dictionary<string, object>
        {
            { "statusId", statusId },
            { "id", item.Id }
        });
        
        await connection.ExecuteAsync("INSERT INTO scraper_queue_item_status_changes (queue_item_id, user_id, from_status_id, to_status_id, reason, changed_at) VALUES (@queueItemId, @userId, @fromId, @toId, @reason, NOW());", new
        {
            item.Id,
            I = 1,
            item.Status,
            statusId,
            reason
        });

        item.Status = statusId;
    }
}