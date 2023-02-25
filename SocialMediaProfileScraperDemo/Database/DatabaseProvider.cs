using System.Data.Common;
using MySqlConnector;

namespace SocialMediaProfileScraperDemo.Database;

public class DatabaseProvider : IDatabaseProvider
{
    private readonly string _connectionString;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseProvider(MySqlConnectionStringBuilder connectionString, IServiceProvider serviceProvider)
    {
        _connectionString = connectionString.ToString();
        _serviceProvider = serviceProvider;
    }

    public DbConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}