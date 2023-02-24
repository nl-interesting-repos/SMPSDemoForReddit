using OpenQA.Selenium;

namespace SocialMediaProfileScraperDemo.Storage;

public interface IStorageClient
{
    Task<string> UploadRemoteFileAsync(string url, string directory, string fileName, bool publicRead);
    Task<string> UploadStreamAsync(Stream fileStream, string directory, string fileName, bool publicRead);
}