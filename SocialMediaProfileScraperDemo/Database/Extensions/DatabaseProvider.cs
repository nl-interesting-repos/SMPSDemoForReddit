using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace SocialMediaProfileScraperDemo.Database.Extensions;

public class DatabaseProvider : IDatabaseProvider
{
    private readonly string _connectionString;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseProvider(MySqlConnectionStringBuilder connectionString, IServiceProvider serviceProvider)
    {
        _connectionString = connectionString.ToString();
        _serviceProvider = serviceProvider;
    }

    public IDatabaseConnection GetConnection()
    {
        var connection = new MySqlConnection(_connectionString);

        return ActivatorUtilities.CreateInstance<DatabaseConnection>(
            _serviceProvider, 
            connection, 
            connection.CreateCommand());
    }
}