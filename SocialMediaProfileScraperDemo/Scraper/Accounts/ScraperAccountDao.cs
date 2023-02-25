using Dapper;
using SocialMediaProfileScraperDemo.Database;

namespace SocialMediaProfileScraperDemo.Scraper.Accounts;

public class ScraperAccountDao
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly DelaySettings _delaySettings;

    public ScraperAccountDao(IDatabaseProvider databaseProvider, DelaySettings delaySettings)
    {
        _databaseProvider = databaseProvider;
        _delaySettings = delaySettings;
    }

    public async Task<ScraperAccount?> GetAccountForHostAsync(string host)
    {
        await using var connection = _databaseProvider.GetConnection();

        await using (var transaction = await connection.BeginTransactionAsync())
        {
            var parameters = new
            {
                host
            };

            var reader = await connection.ExecuteReaderAsync(
                @$"
                SELECT @id := id AS id, username, password, host, status_id FROM `scraper_accounts` 
                WHERE `host` = @host AND 
                      (`fetched_at` IS NULL OR `fetched_at` < (NOW() - INTERVAL {_delaySettings.SecondsToBlockAccountAfterFetched} SECOND)) AND status_id = 1
                ORDER BY `fetched_at` ASC LIMIT 1 FOR UPDATE;
                UPDATE `scraper_accounts` SET `fetched_at` = NOW() WHERE `id` = @id;", parameters, transaction);

            await transaction.CommitAsync();

            while (await reader.ReadAsync())
            {
                return new ScraperAccount(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    (ScraperAccountStatus) reader.GetInt32(4));
            }

            return null;
        }
    }

    public async Task UpdateStatusForAccountAsync(ScraperAccount account, ScraperAccountStatus newStatusId, string reason)
    {
        await using var connection = _databaseProvider.GetConnection();
        const string updateStatusQuery = "UPDATE scraper_accounts SET status_id = @statusId WHERE id = @id;";

        var statusId = (int)newStatusId;

        await connection.ExecuteAsync(updateStatusQuery, new
        {
            statusId,
            account.Id
        });

        account.Status = newStatusId;
    }

    public async Task LogStatusChangeAsync(ScraperAccount account, ScraperAccountStatus newStatusId, string reason)
    {
        await using var connection = _databaseProvider.GetConnection();
        const string insertStatusChangeQuery = "INSERT INTO scraper_queue_item_status_changes (account_id, from_status_id, to_status_id, reason, changed_at) VALUES (@accountId, @fromId, @toId, @reason, NOW());";

        var statusId = (int)newStatusId;

        await connection.ExecuteAsync(insertStatusChangeQuery, new
        {
            account.Id,
            account.Status,
            statusId,
            reason
        });
    }
}