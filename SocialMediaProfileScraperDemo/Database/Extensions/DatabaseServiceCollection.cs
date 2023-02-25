using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace SocialMediaProfileScraperDemo.Database.Extensions;

public class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Default connection string is null or empty");
        }
        
        serviceCollection.AddSingleton(new MySqlConnectionStringBuilder(connectionString));
        serviceCollection.AddTransient<IDatabaseConnection, DatabaseConnection>();
        serviceCollection.AddSingleton<IDatabaseProvider, DatabaseProvider>();
    }
}