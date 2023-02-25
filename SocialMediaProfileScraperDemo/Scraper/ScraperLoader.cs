using SocialMediaProfileScraperDemo.Queue;
using SocialMediaProfileScraperDemo.Scraper.Extractors;
using SocialMediaProfileScraperDemo.Scraper.Presenters;
using SocialMediaProfileScraperDemo.Storage;

namespace SocialMediaProfileScraperDemo.Scraper;

public class ScraperLoader
{
    private readonly Dictionary<string, IScraperDataPresenter> _presenters;
    private readonly Dictionary<string, IScraperDataExtractor> _extractors;
    private readonly IStorageClient _storageClient;

    public ScraperLoader(
        Dictionary<string, IScraperDataPresenter> presenters,
        Dictionary<string, IScraperDataExtractor> extractors,
        IStorageClient storageClient)
    {
        _presenters = presenters;
        _extractors = extractors;
        _storageClient = storageClient;
    }

    public async Task<ScraperLoaderResult> GetResultForQueueItemAsync(QueueItem item)
    {
        var host = new Uri(item.Url).Host;
        var presenter = _presenters[host];
        var extractor = _extractors[host];

        await presenter.PresentDataForItemAsync(item);

        // Take a screenshot of the profile and upload it
        var cdnFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";
        var screenshotUrl = await _storageClient.UploadByteArrayAsync(extractor.GetScreenshotBytes(), $"queue-items/screenshots", cdnFileName, true);

        // Platform links typically include a hash that expires after a specific period of time
        // Uploading to our own storage to ensure we persist for the foreseeable future.
        var sourcePicture = extractor.GetDisplayPicture();
        var cdnPicture = await _storageClient.UploadRemoteFileAsync(sourcePicture, $"queue-items/pictures", cdnFileName, true);

        return new ScraperLoaderResult(200, new QueueItemData(
            item.Id,
            screenshotUrl,
            extractor.GetDisplayName(),
            extractor.GetUsername(),
            cdnPicture,
            extractor.GetBiography(),
            extractor.IsPrivate(),
            extractor.GetOtherData(),
            DateTime.Now
        ));
    }
}