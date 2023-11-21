using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;
using ApplicationLibrary.Data.Repositories.Database;

namespace ApplicationLibrary.Tests.Data.Repositories;

[TestFixture]
public class CourseRepositoryDatabaseTests
{
    [Test, Timeout(30_000), Order(1)]
    [Description("Tests if we can connect to the specified database within the timeout time.")]
    public void ConnectionTest()
    {
        IRepository<Course> courseRepo = new CourseRepositoryDatabase(AppSettings.DbConnectionString);
        
        if (courseRepo.IsValid())
            Assert.Pass();
        else
            Assert.Fail();
    }
    
    [Test, Timeout(30_000), Order(2)]
    [Description("Prints out the 'Course' objects obtained from the database.")]
    public void PrintAll()
    {
        IRepository<Course> courseRepo = new CourseRepositoryDatabase(AppSettings.DbConnectionString);
        var courses = courseRepo.GetAll().ToList();
        foreach (var course in courses)
        {
            Console.WriteLine($"{course}\n\t->{course.Description}");
            Console.WriteLine(string.Join("\n", course.Prerequisites));
            Console.WriteLine("\n");
        }
        
        Assert.Pass();
    }
}