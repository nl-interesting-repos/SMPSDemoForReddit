using Microsoft.Extensions.Configuration;
using SocialMediaProfileScraperDemo.Scraper.Browser;
using SocialMediaProfileScraperDemo.Storage;

namespace SocialMediaProfileScraperDemo.Queue;

public class QueueItemDataLoader
{
    private readonly ScraperBrowser _browser;
    private readonly StorageClient _storageClient;
    private readonly IConfiguration _configuration;

    public QueueItemDataLoader(ScraperBrowser browser, StorageClient storageClient, IConfiguration configuration)
    {
        _browser = browser;
        _storageClient = storageClient;
        _configuration = configuration;
    }

    public async Task<QueueItemLoaderResult> LoadDataForItem(QueueItem item)
    {
        if (_configuration.GetValue<bool>("BrowserSettings:Authentication"))
        {
            var browserAuthenticator = _browser.GetAuthenticatorForHost(new Uri(item.Url).Host.Replace("www.", ""));
            await browserAuthenticator.AuthIfNeededAsync();
        }

        _browser.NavigateToUrl(item.Url, true);

        var browserReader = _browser.GetReaderForHost();
        
        browserReader.WaitForProfileLoad();
        
        var screenshot = _browser.GetScreenshot();
        var fileName = $"{item.Id}.png";
        
        screenshot.SaveAsFile(fileName);

        var cdnDirectory = "queue-items";
        var cdnFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";
        var screenshotUrl = await _storageClient.UploadLocalFileAsync(File.OpenRead(fileName), $"{cdnDirectory}/screenshots", cdnFileName, true);
        
        File.Delete(fileName);

        var sourcePicture = browserReader.GetPictureForProfile();
        var cdnPicture = await _storageClient.UploadFileAsync(sourcePicture, $"{cdnDirectory}/pictures", cdnFileName, true);

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