using MySqlConnector;

namespace SocialMediaProfileScraperDemo.Database.Extensions;

public interface IDatabaseConnection : IDisposable
{
    void SetQuery(string commandText);
    Task<int> ExecuteNonQueryAsync();
    Task<MySqlDataReader> ExecuteReaderAsync();
    void AddParameters(Dictionary<string, object> parameters);
    Task<T> ExecuteScalarAsync<T>();
}