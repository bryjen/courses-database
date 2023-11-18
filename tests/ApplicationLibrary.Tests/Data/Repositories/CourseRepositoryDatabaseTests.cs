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
        courseRepo.GetAll().ToList()
            .ForEach(course => Console.WriteLine($"{course}\n\t->{course.Description}\n\t->{course.Components}\n\t->{course.Notes}\n"));
        
        Assert.Pass();
    }
}