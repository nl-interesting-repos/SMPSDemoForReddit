using Microsoft.Extensions.Configuration;
using SocialMediaProfileScraperDemo.Scraper.Browser;
using SocialMediaProfileScraperDemo.Storage;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemDataLoader
{
    private readonly ScraperBrowser _browser;
    private readonly IStorageClient _storageClient;
    private readonly IConfiguration _configuration;

    public QueueItemDataLoader(ScraperBrowser browser, IStorageClient storageClient, IConfiguration configuration)
    {
        _browser = browser;
        _storageClient = storageClient;
        _configuration = configuration;
    }

    public async Task<QueueItemLoaderResult> LoadDataForItemAsync(QueueItem item)
    {
        if (_configuration.GetValue<bool>("BrowserSettings:Authentication"))
        {
            var browserAuthenticator = _browser.GetAuthenticatorForHost(new Uri(item.Url).Host.Replace("www.", ""));
            await browserAuthenticator.AuthIfNeededAsync();
        }

        _browser.NavigateToUrl(item.Url, true);

        var browserReader = _browser.GetReaderForHost();
        
        browserReader.WaitForProfileLoad();
        
        var cdnFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";

        var screenshot = _browser.GetScreenshot();
        var screenshotUrl = await _storageClient.UploadScreenshotAsync(screenshot, $"queue-items/screenshots", cdnFileName, true);

        var sourcePicture = browserReader.GetPictureForProfile();
        var cdnPicture = await _storageClient.UploadRemoteFileAsync(sourcePicture, $"queue-items/pictures", cdnFileName, true);

        return new QueueItemLoaderResult(200, new QueueItemData(
            item.Id, 
            screenshotUrl,
            browserReader.GetDisplayNameForProfile(), 
            browserReader.GetUsernameForProfile(), 
            cdnPicture,
            browserReader.GetBiographyForProfile(), 
            browserReader.IsProfilePrivate(), 
            browserReader.GetOtherDataForProfile(),
            DateTime.Now
        ));
    }
}