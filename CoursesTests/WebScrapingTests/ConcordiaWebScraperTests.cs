using System.Diagnostics;
using UniversityDatabase.Data.Web;
using UniversityDatabase.Data.Web.Concordia;

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
        WebScraper cws = new ConcordiaWebScraper();
        var rawData = cws.ScrapeAll(52, 2);
        // var rawData = cws.ScrapeAll(52, 2);
        // rawData.ForEach(data => Console.WriteLine(data + "\n"));

        cws.TransformToCourse(rawData[0]);
        //var courses = rawData.Select(str => cws.TransformToCourse(str)).ToList();

        Assert.Pass();
    }
}