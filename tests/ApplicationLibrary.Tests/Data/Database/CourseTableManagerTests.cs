using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Database;
using ApplicationLibrary.Data.WebScraping;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationLibrary.Tests.Data.Database;

public class CourseTableManagerTests
{
/*  //  Initializes the 'courses-prerequisites' table in the database.
    [SetUp]
    public void SetUp()
    {
        var prereqCourseTableManager = new PrereqCourseTableManager(AppSettings.DbConnectionString);
        var webScraper = new ConcordiaWebScraper("./Config/files/concordia_urls.json");
        
        var prerequisiteCourseData = webScraper.ScrapeAll()
            .Select(str => webScraper.TransformToCourse(str))
            .Select(course => course.GetPrerequisiteCourseData())
            .Where(prereq => !prereq.PrerequisitesString.IsNullOrEmpty())
            .Select(prereq => prereq.SplitPrerequisiteCourseData())
            .SelectMany(splitPrereqs => splitPrereqs);

        prereqCourseTableManager.ReplaceAllWith(prerequisiteCourseData);
    }
 */
    
    [Test]
    public void SampleTest()
    {
        Assert.Pass();
    }
}