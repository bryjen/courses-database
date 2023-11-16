using System.Text.Json;
using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;

namespace ApplicationLibrary.Tests;

public class Runner
{
    [Ignore("")]
    [Test]
    public void Run1()
    {
        IRepository<Course> courseRepo = new CourseRepositoryDatabase(AppSettings.DbConnectionString);
        IEnumerable<Course> courses = courseRepo.GetAll();

        var options = new JsonSerializerOptions { WriteIndented = true };
        
        string rawJson = JsonSerializer.Serialize(courses.ToList(), options);
        string fileName = $"courses_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}";
        Console.WriteLine(fileName);

        File.WriteAllText($"{AppSettings.SolutionDirectory}/repo/{fileName}.json", rawJson);
    }
    
    [Test]
    public void Run2()
    {
        var filePath = LocalFileSelector.GetSerializedCoursesFileName();

        if (filePath is null)
        {
            Console.WriteLine("Filepath is null");
            Assert.Fail();
            return;
        }

        var rawJson = File.ReadAllText(filePath);
        var deserializedData = JsonSerializer.Deserialize<Course[]>(rawJson);

        //  Console.WriteLine(rawJson);
        
        if (deserializedData is null)
            return;
            
        var courses = deserializedData.ToList();
        
        
        courses.ToList().ForEach(Console.WriteLine);
    }
}