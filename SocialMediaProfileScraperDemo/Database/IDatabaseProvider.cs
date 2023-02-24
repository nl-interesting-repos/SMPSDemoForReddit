namespace SocialMediaProfileScraperDemo.Database;

public interface IDatabaseProvider
{
    IDatabaseConnection GetConnection();
}