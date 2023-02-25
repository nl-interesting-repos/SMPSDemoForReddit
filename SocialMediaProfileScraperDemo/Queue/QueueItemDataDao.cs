using Dapper;
using Newtonsoft.Json;
using SocialMediaProfileScraperDemo.Database;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemDataDao
{
    private readonly IDatabaseProvider _databaseProvider;

    public QueueItemDataDao(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
    }

    public async Task StoreAsync(QueueItemData queueItemData)
    {
        var otherData = JsonConvert.SerializeObject(queueItemData.OtherData);

        await using var connection = _databaseProvider.GetConnection();
        const string query = "INSERT INTO `scraper_queue_item_data` (queue_item_id, screenshot_url, display_name, username, picture, biography, is_private, other_data, loaded_at) VALUES (@itemId, @screenshot, @name, @username, @picture, @bio, @isPrivate, @otherData, @loadedAt);";

        await connection.ExecuteAsync(query, new
        {
            queueItemData.ItemId,
            queueItemData.ScreenshotUrl,
            queueItemData.DisplayName,
            queueItemData.Username,
            queueItemData.Picture,
            queueItemData.Biography,
            queueItemData.IsPrivate,
            otherData,
            queueItemData.LoadedAt,
        });
    }
}