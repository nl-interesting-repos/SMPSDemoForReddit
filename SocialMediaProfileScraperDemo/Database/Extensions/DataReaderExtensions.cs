using System.Data;

namespace CascadeQueueItemAnalyzer.Database.Extensions;

public static class DataReaderExtensions
{
    public static IEnumerable<Dictionary<string, object>> ToRecordsCollection(this IDataReader reader)
    {
        var records = new List<Dictionary<string, object>>();

        while (reader.Read())
        {
            var record = Enumerable.Range(0, reader.FieldCount)
                .ToDictionary(reader.GetName, reader.GetValue);
            
            records.Add(record);
        }

        return records;
    }
}