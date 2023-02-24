using System.Globalization;
using MySqlConnector;

namespace SocialMediaProfileScraperDemo.Database;

public class DatabaseConnection : IDatabaseConnection
{
    private readonly MySqlConnection _connection;
    private readonly MySqlCommand _command;

    public DatabaseConnection(MySqlConnection connection, MySqlCommand command)
    {
        _connection = connection;
        _command = command;

        _connection.Open();
    }

    public void SetQuery(string commandText)
    {
        _command.Parameters.Clear();
        _command.CommandText = commandText;
    }
    
    public async Task<int> ExecuteNonQueryAsync()
    {
        return await _command.ExecuteNonQueryAsync();
    }

    public async Task<MySqlDataReader> ExecuteReaderAsync()
    {
        return await _command.ExecuteReaderAsync();
    }

    private void AddParameter(string name, object value)
    {
        var parameter = _command.CreateParameter();
            
        parameter.ParameterName = $"@{name}";
        parameter.Value = value;

        _command.Parameters.Add(parameter);
    }

    public void AddParameters(Dictionary<string, object> parameters)
    {
        foreach (var (key, value) in parameters)
        {
            AddParameter(key, value);
        }
    }
        
    public async Task<T> ExecuteScalarAsync<T>()
    {
        return (T) Convert.ChangeType(await _command.ExecuteScalarAsync(), typeof(T), CultureInfo.InvariantCulture)!;
    }
        
    public void Dispose()
    {
        _connection.Close();
        _command.Dispose();
    }
}