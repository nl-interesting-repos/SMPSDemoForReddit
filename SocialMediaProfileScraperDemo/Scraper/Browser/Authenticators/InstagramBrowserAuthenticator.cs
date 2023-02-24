using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SocialMediaProfileScraperDemo.Scraper.Accounts;
using SocialMediaProfileScraperDemo.Storage;

namespace SocialMediaProfileScraperDemo.Scraper.Browser.Authenticators;

public class InstagramBrowserAuthenticator : IScraperBrowserAuthenticator
{
    private readonly ILogger<InstagramBrowserAuthenticator> _logger;
    private readonly ScraperBrowser _browser;
    private readonly ScraperAccountRepository _accountRepository;
    private readonly DelaySettings _delaySettings;
    private readonly IStorageClient _storageClient;

    public InstagramBrowserAuthenticator(
        ILogger<InstagramBrowserAuthenticator> logger,
        ScraperBrowser browser,
        ScraperAccountRepository accountRepository,
        DelaySettings delaySettings,
        IStorageClient storageClient)
    {
        _logger = logger;
        _browser = browser;
        _accountRepository = accountRepository;
        _delaySettings = delaySettings;
        _storageClient = storageClient;
    }

    private ScraperAccount _currentAccount;
    private bool _authenticated;

    public async Task AuthIfNeededAsync()
    {
        while (!_authenticated || NeedsSessionRenewal())
        {
            _logger.LogInformation("Authenticating...");

            if (_authenticated)
            {
                Logout();
            }

            _browser.NavigateToUrl("https://instagram.com/accounts/login");
            _browser.WaitForElementToAppear(By.Name("username"), TimeSpan.FromSeconds(10));

            AllowCookies();

            var account = await _accountRepository.GetAccountForHostAsync(_browser.Host);

            if (account == null)
            {
                _logger.LogInformation("Failed to fetch an account, waiting and retrying...");
                Thread.Sleep(TimeSpan.FromSeconds(_delaySettings.SecondsToWaitAfterAccountFetchFailed));
                continue;
            }
            
            if (!await TryLoginWithCredentialsAsync(account.Username, account.Password))
            {
                _logger.LogInformation("Failed to authenticate with this account, waiting and retrying...");
                Thread.Sleep(TimeSpan.FromSeconds(_delaySettings.SecondsToWaitAfterAccountLoginFailed));
                continue;
            }

            if (!await VerifyLoginAsync())
            {
                Logout();
                continue;
            }

            _profilePageLoads = 0;
            _authenticated = true;
            _currentAccount = account;
        }
    }

    private async Task<bool> VerifyLoginAsync()
    {
        _browser.NavigateToUrl(_browser.Url);
        
        if (_browser.Url.Contains("instagram.com/challenge/") || _browser.Url.Contains("instagram.com/accounts/suspended"))
        {
            _browser.GetScreenshot();

            var screenshot = _browser.GetScreenshot();

            var cdnFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";
            var cdnDirectory = "accounts/screenshots";
            var screenshotUrl = await _storageClient.UploadScreenshotAsync(screenshot, cdnDirectory, cdnFileName, true);

            await _accountRepository.UpdateStatusAsync(_currentAccount, ScraperAccountStatus.BeyondRepair, screenshotUrl);

            return false;
        }

        var rootHtmlTag = _browser.GetDomElement(By.TagName("html"));

        if (rootHtmlTag == null)
        {
            throw new Exception("The root HTML tag couldn't be found in the dom");
        }
        
        return !rootHtmlTag.GetAttribute("className").Contains("not-logged-in");
    }

    private void AllowCookies()
    {
        var cookieButton = _browser.GetDomElementByXPath("//button[text()='Allow essential and optional cookies']");

        if (cookieButton != null)
        {
            cookieButton.Click();
            _browser.WaitForSourceCodePhraseToDisappear("Allow essential and optional cookies", TimeSpan.FromSeconds(5));
        }
    }

    private int _profilePageLoads;
    
    public void IncrementProfilePageLoadCountForSession()
    {
        _profilePageLoads++;
    }

    private void WaitTillLoginSubmitted(TimeSpan timeOut)
    {
        var started = DateTime.Now;

        while (true)
        {
            if (!_browser.PageSource.Contains("loading-state") || (DateTime.Now - started) >= timeOut)
            {
                break;
            }

            Thread.Sleep(200);
        }
    }

    private async Task<bool> TryLoginWithCredentialsAsync(string username, string password)
    {
        var usernameField = _browser.GetDomElement(By.Name("username"));
        var passwordField = _browser.GetDomElement(By.Name("password"));

        if (usernameField == null || passwordField == null)
        {
            return false;
        }

        var random = new Random();

        usernameField.SendKeys(username);
        Thread.Sleep(random.Next(500, 1000) * 8);
        passwordField.SendKeys(password);
        Thread.Sleep(random.Next(500, 1000) * 8);
        passwordField.SendKeys(Convert.ToString(Convert.ToChar((object)57351)));

        WaitTillLoginSubmitted(TimeSpan.FromSeconds(10));

        var authError = GetAuthenticationError();

        if (!string.IsNullOrEmpty(authError))
        {
            await _accountRepository.UpdateStatusAsync(_currentAccount, ScraperAccountStatus.NeedsReviewing, authError);
            return false;
        }

        return true;
    }

    private string? GetAuthenticationError()
    {
        return _browser.GetDomElement(By.Id("slfErrorAlert"))?.GetAttribute("innerText");
    }

    private bool NeedsSessionRenewal()
    {
        return _profilePageLoads >= 50;
    }

    private void Logout()
    {
        _logger.LogInformation("Logging out...");
        _browser.NavigateToUrl("https://instagram.com/accounts/logout");
    }
}
