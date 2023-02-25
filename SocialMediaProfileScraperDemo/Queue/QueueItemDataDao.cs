using Newtonsoft.Json;
using SocialMediaProfileScraperDemo.Database;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemDataDao : BaseDao
{
    public QueueItemDataDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
    }

    public async Task StoreAsync(QueueItemData queueItemData)
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