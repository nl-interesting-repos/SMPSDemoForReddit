using SocialMediaProfileScraperDemo.Scraper.Accounts;

public record ScraperAccount(int Id, string Username, string Password, string Host, ScraperAccountStatus Status)
{
    public ScraperAccountStatus Status { get; set; } = Status;
}