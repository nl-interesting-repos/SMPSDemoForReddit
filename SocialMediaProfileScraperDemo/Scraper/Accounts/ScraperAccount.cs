namespace SocialMediaProfileScraperDemo.Scraper.Accounts;

public class ScraperAccount
{
    public int Id { get; }
    public string Username { get; }
    public string Password { get; }
    public string Host { get; }
    public ScraperAccountStatus Status { get; set; }

    public ScraperAccount(int id, string username, string password, string host, ScraperAccountStatus status)
    {
        Id = id;
        Username = username;
        Password = password;
        Host = host;
        Status = status;
    }
}