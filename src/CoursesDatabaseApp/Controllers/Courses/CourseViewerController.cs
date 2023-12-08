using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace CoursesDatabaseApp.Controllers.Courses;

public class CourseViewerController : Controller
{
    private readonly IMemoryCache _cache;

    public CourseViewerController(IMemoryCache cache)
    {
        this._cache = cache;
    }

    //  Example request:
    //  .../CourseViewer/GetCoursesByType?courseType=COMP
    [HttpGet("CourseViewer/GetCoursesByType")]
    public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByType(string courseType)
    {
        var result = await Task.Run(() =>
        {
            courseType = courseType.ToLower().Trim();
            
            //  Try getting from cache
            if (!_cache.TryGetValue($"{courseType}_courses", out List<Course>? courses))
            {
                using CourseRepository courseRepository = new CourseRepository();
                courses = courseRepository.GetCoursesByType(1, courseType);

                if (courses.IsNullOrEmpty())
                    return null;
                
                _cache.Set($"{courseType}_courses", courses, TimeSpan.FromMinutes(10));
            }

            return courses;
        });

        return result is not null 
            ? result 
            : BadRequest();
    }

    public async Task<IActionResult> CoursesCatalog()
    {
        return await Task.Run(() =>
        {
            //  Try getting from cache
            if (!_cache.TryGetValue("comp_courses", out List<Course>? courses))
            {
                using CourseRepository courseRepository = new CourseRepository();
                courses = courseRepository.GetCoursesByType(1, "comp");
                
                _cache.Set("comp_courses", courses, TimeSpan.FromMinutes(10));
            }
            
            return View(courses);
        });
    }
    
    public async Task<IActionResult> CourseProfile(int courseUniversityId, string courseType, int courseNumber)
    {
        return await Task.Run(() =>
        {
            //  We can generate a 'course signature' from the details given. This is a unique identifier for a course
            //  in the database.
            string courseSignature = $"{courseUniversityId} {courseType.ToLower()} {courseNumber}";

            //  Try getting from cache
            if (!_cache.TryGetValue(courseSignature, out Course? selectedCourse))
            {
                using CourseRepository courseRepository = new CourseRepository();
                selectedCourse = courseRepository.TryGetCourse(courseUniversityId, courseType, courseNumber);
                
                if (selectedCourse is null)
                    return (IActionResult) BadRequest($"Could not find the course with id: {courseUniversityId}, type: " +
                                                      $"{courseType}, number: {courseNumber}");

                _cache.Set(courseSignature, selectedCourse, TimeSpan.FromMinutes(10));
            }

            return View(selectedCourse);
        });
    }
}