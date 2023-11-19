using System.Text.Json;
using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories.Database;
using ApplicationLibrary.Data.WebScraping;

namespace ApplicationLibrary.Tests;

public class Runner
{
    [Test]
    public void Run1()
    {
        WebScraper webScraper = new ConcordiaWebScraper("Config/files/concordia_urls.json");
        var rawCourses = webScraper.ScrapeAll(52, 2);
        var courses = rawCourses.Select(rawHtml => webScraper.TransformToCourse(rawHtml)).ToList();
        string rawJson = JsonSerializer.Serialize(courses, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText($"{AppSettings.SolutionDirectory}/repo/courses_2023_11_19.json", rawJson);
        
        
        //string rawJson = File.ReadAllText($"{AppSettings.SolutionDirectory}/repo/courses_2023_11_19.json");
        //List<Course> courses = JsonSerializer.Deserialize<List<Course>>(rawJson) ?? new List<Course>();

        // CourseRepositoryDatabase courseTableManager = new CourseRepositoryDatabase(AppSettings.DbConnectionString);
        // Console.WriteLine(courseTableManager.ReplaceAllCourses(courses));

        //  courses.ForEach(course =>
        //  {
        //      Console.WriteLine($"{course}\n\t->{course.Description}\n");
        //      (course.Components ?? new List<string>()).ToList().ForEach(component => Console.WriteLine($"component: {component}"));
        //      (course.Notes ?? new List<string>()).ToList().ForEach(note => Console.WriteLine($"note: {note}"));
        //  });

    }
}