using System.Diagnostics;
using CoursesDatabase.Config;
using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;
using Microsoft.Data.SqlClient;

namespace CoursesDatabase.Tests.UnitTests.Data.Repositories;

[TestFixture]
public class CourseRepositoryTests
{
    //  Ensures that we can connect to the database.
    [OneTimeSetUp]
    public void TestConnection()
    {
        using SqlConnection dbConnection = new SqlConnection(DatabaseSettings.DbConnectionString);

        try
        {
            dbConnection.Open();
            Console.WriteLine("Connection successful.");
        }
        catch
        {
            throw;
        }
        finally
        {
            dbConnection.Close();
        }
    }
    
    [Retry(3)]
    [TestCase(1, "comp", 228, true)]
    [TestCase(1, "comp", 22, false)]
    public void TestTryGetCourse(int courseUniversityId, string courseType, int courseNumber, bool existsInTheDb)
    {
        using CourseRepository courseRepository = new CourseRepository();
        Course? selectedCourse = courseRepository.TryGetCourse(courseUniversityId, courseType, courseNumber);
        Assert.That(
            selectedCourse, 
            existsInTheDb ? Is.Not.Null : Is.Null, 
            $"Expected {(existsInTheDb ? "NOT " : "")}to find course w/ signature \"{courseUniversityId} {courseType} {courseNumber}\". FAILED.");

        Console.WriteLine("Success.");
    }
}