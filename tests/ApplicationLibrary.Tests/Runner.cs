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
        string rawJson = File.ReadAllText($"{AppSettings.SolutionDirectory}/repo/courses_2023_11_19.json");
        List<Course> courses = JsonSerializer.Deserialize<List<Course>>(rawJson) ?? new List<Course>();

        var courseRepoDatabase = new CourseRepositoryDatabase(AppSettings.DbConnectionString);

    }
}