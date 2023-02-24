namespace SocialMediaProfileScraperDemo.Scraper.Accounts;

public class ScraperAccountRepository
{
    private readonly ScraperAccountDao _accountDao;

    public ScraperAccountRepository(ScraperAccountDao accountDao)
    {
        _accountDao = accountDao;
    }
    
    public async Task<ScraperAccount?> GetAccountForHostAsync(string host)
    {
        return await _accountDao.GetAccountForHostAsync(host);
    }

    public async Task UpdateStatusAsync(ScraperAccount account, ScraperAccountStatus newStatus, string reason)
    {
        await _accountDao.UpdateStatusForAccountAsync(account, newStatus, reason);
    }
}