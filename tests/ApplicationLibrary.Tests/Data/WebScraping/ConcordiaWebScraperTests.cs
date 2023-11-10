using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.WebScraping;

namespace ApplicationLibrary.Tests.Data.WebScraping;

public class ConcordiaWebScraperTests
{
    //  If you change the .json file in the main application library, DONT FORGET to change the .json file in this project too.
    [TestCase(52, 2, @"Config/files/concordia_urls.json")]
    [Description("Prints out the 'Course' objects web-scraped.")]
    public void PrintAll(int startIndex, int number, string filePath)
    {
        WebScraper concordiaWebScraper = new ConcordiaWebScraper(filePath);
        List<string> rawData = concordiaWebScraper.ScrapeAll(startIndex, number);
        List<Course> courses = rawData.Select(concordiaWebScraper.TransformToCourse).ToList();
        
        courses.ForEach(course => Console.WriteLine($"{course}\n\t->{course.Description}\n\t->{course.Components}\n\t->{course.Notes}\n"));

        Assert.Pass();
    }
}