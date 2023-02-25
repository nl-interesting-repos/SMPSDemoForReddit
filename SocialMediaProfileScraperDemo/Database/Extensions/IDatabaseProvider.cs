namespace SocialMediaProfileScraperDemo.Database.Extensions;

public interface IDatabaseProvider
{
    IDatabaseConnection GetConnection();
}