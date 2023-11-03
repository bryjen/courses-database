using UniversityDatabase.Core;

namespace CoursesTests;

#if !TEST_CONNECTION
[Ignore("Azure SQL server not connecting.")]
#endif

[TestFixture]
public class CoursesTests
{
    [SetUp]
    public void SetUp()
    {
        List<Course> courses = Course.GetCourses();
        courses.ForEach(Console.WriteLine);
    }

    [Test]
    public void PrintContents()
    {
        Assert.Pass();
    }
}