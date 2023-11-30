using CoursesDatabase.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CoursesDatabaseApi.Controllers.Course;

[ApiController]
[Route("[controller]")]
public class CourseViewerController : Controller
{
    [HttpGet("GetCourse")]
    public ActionResult<Models.Course> TryGetCourse(int courseUniversityId, string courseName,
        int courseNumber)
    {
        using CourseRepository courseRepository = new CourseRepository();
        Models.Course? selectedCourse = courseRepository.TryGetCourse(courseUniversityId, courseName, courseNumber);
        return (selectedCourse is null) ? Empty : selectedCourse;
    }

    [HttpGet("GetAllCourses/Initialized")]
    public IEnumerable<Models.Course> GetAllCoursesInitialized()
    {
        using CourseRepository courseRepository = new CourseRepository();
        return courseRepository.GetAllCourses();
    }
    
    [HttpGet("GetAllCourses/Uninitialized")]
    public IEnumerable<Models.Course> GetAllCoursesUninitialized()
    {
        using CourseRepository courseRepository = new CourseRepository();
        IEnumerable<Models.Course> courses = courseRepository
            .GetAllCoursesUninitialized();

        return Models.Course.TrimData(courses);
    }
}