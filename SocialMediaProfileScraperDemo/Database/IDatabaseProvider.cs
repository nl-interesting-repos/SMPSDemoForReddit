using System.Data.Common;

namespace SocialMediaProfileScraperDemo.Database;

public interface IDatabaseProvider
{
    DbConnection GetConnection();
}