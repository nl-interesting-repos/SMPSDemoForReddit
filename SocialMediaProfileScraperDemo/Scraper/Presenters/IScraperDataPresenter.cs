using SocialMediaProfileScraperDemo.Queue;

namespace SocialMediaProfileScraperDemo.Scraper.Presenters;

public interface IScraperDataPresenter
{
    Task PresentDataForItemAsync(QueueItem item);
}