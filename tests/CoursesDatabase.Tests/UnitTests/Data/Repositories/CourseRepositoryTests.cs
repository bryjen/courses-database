using CoursesDatabase.Config;
using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;
using Microsoft.Data.SqlClient;

namespace CoursesDatabase.Tests.UnitTests.Data.Repositories;

[TestFixture]
[NonParallelizable]
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
    
    /// <summary>
    /// Tests <see cref="CourseRepository.TryGetCourse"/>
    /// </summary>
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

        //  Additional testing to ensure that the selected course matches the specified parameters.
        if (selectedCourse is not null)
        {
            Assert.That(
                selectedCourse.UniversityId, 
                Is.EqualTo(courseUniversityId),
                $"University Ids do not match.\nExpected: {courseUniversityId}\nGot: {selectedCourse.UniversityId}");
        
            Assert.That(
                selectedCourse.Type, 
                Is.EqualTo(courseType),
                $"Course types do not match.\nExpected: {courseType}\nGot: {selectedCourse.Type}");
        
            Assert.That(
                selectedCourse.Number, 
                Is.EqualTo(courseNumber),
                $"Course numbers do not match.\nExpected: {courseNumber}\nGot: {selectedCourse.Number}");
        }
        
        Assert.Pass("Success.");
    }

    /// <summary>
    /// Tests <see cref="CourseRepository.GetCoursesByType"/>.
    /// </summary>
    /// <remarks>
    /// Tests that each entry in the list is of the specified type, NOT that if all courses of the specified type in
    /// the database are in the in the list. 
    /// </remarks>
    [Retry(3)]
    [TestCase(1, "comp")]
    [TestCase(1, "engr")]
    public void TryGetCourseByType(int courseUniversityId, string courseType)
    {
        using CourseRepository courseRepository = new CourseRepository();
        List<Course> courses = courseRepository.GetCoursesByType(courseUniversityId, courseType);

        foreach (Course course in courses)
        {
            Console.WriteLine(course.CourseSignature);
            if (!course.Type.Equals(courseType, StringComparison.InvariantCultureIgnoreCase))
            {
                Assert.Fail($"The specified type was \"{courseType}\", a course of type \"{course.Type}\" was found.");
            }
        }
        
        Assert.Pass("Success.");
    }
    
    
}