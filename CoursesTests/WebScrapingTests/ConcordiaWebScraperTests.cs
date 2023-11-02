using Courses.WebScraping.Concordia;

namespace CoursesTests.WebScrapingTests;


#if !TEST_WEBSCRAPING
[Ignore("Testing of web scrapers disabled.")]
#endif

[TestFixture]
public class ConcordiaWebScraperTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void PrintContents()
    {
        ConcordiaWebScraper cws = new ConcordiaWebScraper();
        cws.Execute();
        Assert.Pass();
    }
}