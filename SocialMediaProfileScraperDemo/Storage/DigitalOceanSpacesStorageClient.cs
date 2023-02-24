using Amazon.S3;
using Amazon.S3.Transfer;
using OpenQA.Selenium;
using SocialMediaProfileScraperDemo.Utilities;

namespace SocialMediaProfileScraperDemo.Storage;

public class DigitalOceanSpacesStorageClient : IStorageClient
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly string _cdnUrl;

    public DigitalOceanSpacesStorageClient(AmazonS3Client s3Client, string bucketName, string cdnUrl)
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
        _cdnUrl = cdnUrl;
    }

    public async Task<string> UploadRemoteFileAsync(string url, string directory, string fileName, bool publicRead)
    {
        var stream = await HttpUtilities.GetStreamFromUrlAsync(url);
        return await UploadStreamAsync(stream, directory, fileName, publicRead);
    }

    public async Task<string> UploadStreamAsync(Stream stream, string directory, string fileName, bool publicRead)
    {
        var transferUtility = new TransferUtility(_s3Client);
            
        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = _bucketName,
            Key = $"{directory}/{fileName}",
            InputStream = stream,
            CannedACL = publicRead ? S3CannedACL.PublicRead : S3CannedACL.Private
        };
            
        await transferUtility.UploadAsync(uploadRequest);
        return $"{_cdnUrl}/{directory}/{fileName}";
    }
}