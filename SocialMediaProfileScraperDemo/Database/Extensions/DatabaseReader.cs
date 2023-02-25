namespace SocialMediaProfileScraperDemo.Database.Extensions;

public class DatabaseReader
{
    private readonly Queue<DatabaseRecord> _records;

    public DatabaseReader(Queue<DatabaseRecord> records)
    {
        _records = records;
    }

    public (bool, DatabaseRecord? record) GetNextRecord()
    {
        return (_records.TryDequeue(out var record), record);
    }
}