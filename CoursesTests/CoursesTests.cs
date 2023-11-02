using Courses;

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
        using (var context = new CourseDataDbContext())
        {
            var allCourses = context.Courses.ToList();
            allCourses.ForEach(Console.WriteLine);
        }
    }

    [Test]
    public void PrintContents()
    {
        Assert.Pass();
    }
}