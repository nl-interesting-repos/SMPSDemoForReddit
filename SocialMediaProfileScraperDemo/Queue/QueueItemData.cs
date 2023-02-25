namespace SocialMediaProfileScraperDemo.Queue;

public record QueueItemData(long ItemId, string ScreenshotUrl, string DisplayName, string Username, string Picture, string Biography, bool IsPrivate, Dictionary<string, object> OtherData, DateTime LoadedAt)
{
    public long ItemId { get; } = ItemId;
    public string ScreenshotUrl { get; } = ScreenshotUrl;
    public string DisplayName { get; } = DisplayName;
    public string Username { get; } = Username;
    public string Picture { get; } = Picture;
    public string Biography { get; } = Biography;
    public bool IsPrivate { get; } = IsPrivate;
    public Dictionary<string, object> OtherData { get; } = OtherData;
    public DateTime LoadedAt { get; } = LoadedAt;
}