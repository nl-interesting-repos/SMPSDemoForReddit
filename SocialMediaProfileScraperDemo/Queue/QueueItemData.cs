namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemData
{
    public QueueItemData(long itemId, string screenshotUrl, string displayName, string username, string picture, string biography, bool isPrivate, Dictionary<string, object> otherData, DateTime loadedAt)
    {
        ItemId = itemId;
        ScreenshotUrl = screenshotUrl;
        DisplayName = displayName;
        Username = username;
        Picture = picture;
        Biography = biography;
        IsPrivate = isPrivate;
        OtherData = otherData;
        LoadedAt = loadedAt;
    }

    public long ItemId { get; set; }
    public string ScreenshotUrl { get; set; }
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public string Picture { get; set; }
    public string Biography { get; set; }
    public bool IsPrivate { get; set; }
    public Dictionary<string, object> OtherData { get; set; }
    public DateTime LoadedAt { get; set; }
}