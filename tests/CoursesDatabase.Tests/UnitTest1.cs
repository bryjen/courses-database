using CoursesDatabase.Config;
using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;

namespace CoursesDatabase.Tests;

public class Tests
{
    [Test]
    public void Runner()
    {
        using CourseRepository courseRepository = new CourseRepository();
        IEnumerable<Course> courses = courseRepository.GetAllCourses();

        foreach (var course in courses)
        {
            Console.WriteLine($"{course.Type} {course.Number} {course.Name} ({course.Credits} credit(s))");
            course.Prerequisites?.ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
        }
    }
}