namespace SocialMediaProfileScraperDemo.Utilities;

public static class HttpUtilities
{
    private static async Task<string> DownloadAsync(string url)
    {
        using var client = new HttpClient();
        return await (await client.GetAsync(url)).Content.ReadAsStringAsync();
    }

    public static async Task<Stream> GetStreamFromUrlAsync(string url)
    {
        using var client = new HttpClient();
        return new MemoryStream(await (await client.GetAsync(url)).Content.ReadAsByteArrayAsync());
    }
}