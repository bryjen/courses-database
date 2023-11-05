using System.Diagnostics;
using UniversityDatabase.Core;
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
        var rawData = cws.ScrapeAll(                  );
        // var rawData = cws.ScrapeAll(52, 2);
        // rawData.ForEach(data => Console.WriteLine(data + "\n"));

        //  cws.TransformToCourse(rawData[0]);
        //  cws.TransformToCourse(rawData[3]);
        var courses = rawData.Select(str => cws.TransformToCourse(str)).ToList();

        Console.WriteLine(Course.AsSqlCommand("[dbo].[courses]", courses));

        Assert.Pass();
    }
}