namespace SocialMediaProfileScraperDemo.Storage;

public interface IStorageClient
{
    Task<string> UploadFileAsync(string url, string directory, string fileName, bool publicRead);
    Task<string> UploadLocalFileAsync(FileStream fileStream, string directory, string fileName, bool publicRead);
}