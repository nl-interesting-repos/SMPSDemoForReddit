using Moq;
using SocialMediaProfileScraperDemo.Scraper.Browser;
using SocialMediaProfileScraperDemo.Scraper.Extractors;

namespace SocialMediaProfileScraperDemo.Tests.Scraper.Extractors;

public class InstagramDataExtractorTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetUsername_Returns_As_Expected()
    {
        var mockBrowser = new Mock<IScraperBrowser>();
        var instagramBrowserReader = new InstagramDataExtractor(mockBrowser.Object);

        mockBrowser.Setup(x => x.Url).Returns("https://instagram.com/username1/");
        Assert.That(instagramBrowserReader.GetUsername(), Is.EqualTo("username1"));

        mockBrowser.Setup(x => x.Url).Returns("https://instagram.com/username2");
        Assert.That(instagramBrowserReader.GetUsername(), Is.EqualTo("username2"));
    }

    [Test]
    public async Task Private_Profile_Detected_As_Private()
    {
        var mockBrowser = new Mock<IScraperBrowser>();

        mockBrowser.Setup(x => x.PageSource).Returns("<p>account is private</p>");

        var instagramBrowserReader = new InstagramDataExtractor(mockBrowser.Object);

        Assert.That(instagramBrowserReader.IsPrivate(), Is.True);
    }

    [Test]
    public async Task Non_Private_Profile_Not_Detected_As_Private()
    {
        var mockBrowser = new Mock<IScraperBrowser>();

        mockBrowser.Setup(x => x.PageSource).Returns("<p>anything else</p>");

        var instagramBrowserReader = new InstagramDataExtractor(mockBrowser.Object);

        Assert.That(instagramBrowserReader.IsPrivate(), Is.False);
    }
}