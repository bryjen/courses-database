using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.WebScraping;

namespace ApplicationLibrary.Tests.Data.WebScraping;

public class ConcordiaWebScraperTests
{
    private const string Filepath = @"Config/files/concordia_urls.json";
    
    //  If you change the .json file in the main application library, DONT FORGET to change the .json file in this project too.
    [TestCase(52, 2)]
    [Description("Prints out the 'Course' objects web-scraped.")]
    public void PrintAll(int startIndex, int number)
    {
        WebScraper concordiaWebScraper = new ConcordiaWebScraper(Filepath);
        List<string> rawData = concordiaWebScraper.ScrapeAll(startIndex, number);
        List<Course> courses = rawData.Select(concordiaWebScraper.TransformToCourse).ToList();
        
        courses.ForEach(course => Console.WriteLine($"{course}\n\t->{course.Description}\n\t->{course.Components}\n\t->{course.Notes}\n"));

        Assert.Pass();
    }
    
    [Test]
    public void TestPrerequisiteInitialization()
    {
        string url = "https://www.concordia.ca/academics/undergraduate/calendar/current/section-71-gina-cody-school-of-engineering-and-computer-science/section-71-70-department-of-computer-science-and-software-engineering/section-71-70-10-computer-science-and-software-engineering-courses.html#3567";

        WebScraper concordiaWebScraper = new ConcordiaWebScraper(Filepath);
        List<string> rawData = concordiaWebScraper.ScrapeWebsite(url);
        List<Course> courses = rawData.Select(concordiaWebScraper.TransformToCourse).ToList();
        
        Assert.Pass();
    }
}