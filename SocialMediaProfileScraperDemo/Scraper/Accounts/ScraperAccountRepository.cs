using SocialMediaProfileScraperDemo.Database;

namespace SocialMediaProfileScraperDemo.Scraper.Accounts;

public class ScraperAccountRepository : BaseDao
{
    private readonly DelaySettings _delaySettings;

    public ScraperAccountRepository(IDatabaseProvider databaseProvider, DelaySettings delaySettings) : base(databaseProvider)
    {
        _delaySettings = delaySettings;
    }

    public async Task<ScraperAccount?> GetAccountForHostAsync(string host)
    {
        var reader = await GetReaderAsync(@$"
            START TRANSACTION;
            SELECT @id := id AS id, username, password, host, status_id FROM `scraper_accounts` 
            WHERE `host` = @host AND 
                  (`fetched_at` IS NULL OR `fetched_at` < (NOW() - INTERVAL {_delaySettings.SecondsToBlockAccountAfterFetched} SECOND)) AND status_id = 1
            ORDER BY `fetched_at` ASC LIMIT 1 FOR UPDATE;
            UPDATE `scraper_accounts` SET `fetched_at` = NOW() WHERE `id` = @id;
            COMMIT;", new Dictionary<string, object>
        {
            { "host", host }
        });

        var (recordExists, accountRecord) = reader.GetNextRecord();

        return recordExists && accountRecord != null ? new ScraperAccount(
            accountRecord.Get<int>("id"),
            accountRecord.Get<string>("username"),
            accountRecord.Get<string>("password"),
            accountRecord.Get<string>("host"),
            (ScraperAccountStatus) accountRecord.Get<int>("status_id")) : null;
    }

    public async Task UpdateStatusAsync(ScraperAccount account, ScraperAccountStatus newStatusId, string reason)
    {
        await QueryAsync("UPDATE scraper_accounts SET status_id = @statusId WHERE id = @id;", new Dictionary<string, object>
        {
            { "statusId", (int) newStatusId },
            { "id", account.Id }
        });

        await QueryAsync("INSERT INTO scraper_queue_item_status_changes (account_id, from_status_id, to_status_id, reason, changed_at) VALUES (@accountId, @fromId, @toId, @reason, NOW());", new Dictionary<string, object>
        {
            { "accountId", account.Id },
            { "fromId", account.Status },
            { "toId", (int) newStatusId },
            { "reason", reason }
        });

        account.Status = newStatusId;
    }
}