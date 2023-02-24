using CascadeQueueItemAnalyzer.Database.Extensions;

namespace SocialMediaProfileScraperDemo.Database;

public class BaseDao
{
    private readonly IDatabaseProvider _databaseProvider;

    protected BaseDao(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
    }

    protected async Task<DatabaseReader> GetReaderAsync(string commandText, Dictionary<string, object>? parameters = null)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        AddParameters(dbConnection, parameters);

        var reader = await dbConnection.ExecuteReaderAsync();
        var batch = reader.ToRecordsCollection();
        
        return new DatabaseReader(
            new Queue<DatabaseRecord>(batch.Select(x => new DatabaseRecord(x)))
        );
    }

    protected async Task<int> QueryAsync(string commandText, Dictionary<string, object>? parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        if (parameters is { Count: > 0 })
        {
            AddParameters(dbConnection, parameters);
        }

        return await dbConnection.ExecuteNonQueryAsync();
    }

    protected async Task<bool> ExistsAsync(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery($"SELECT CASE WHEN EXISTS({commandText.TrimEnd(new []{';'})}) THEN 1 ELSE 0 END");

        AddParameters(dbConnection, parameters);

        return await dbConnection.ExecuteScalarAsync<int>() == 1;
    }

    protected async Task<T> QueryScalarAsync<T>(string commandText, Dictionary<string, object>? parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        if (parameters is { Count: > 0 })
        {
            AddParameters(dbConnection, parameters);
        }

        return await dbConnection.ExecuteScalarAsync<T>();
    }

    private static void AddParameters(IDatabaseConnection connection, Dictionary<string, object> parameters)
    {
        connection.AddParameters(parameters);
    }
}