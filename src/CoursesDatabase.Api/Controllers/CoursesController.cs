using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace CoursesDatabase.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoursesController : Controller
{
    private readonly IMemoryCache _cache;
    
    public CoursesController(IMemoryCache cache)
    {
        this._cache = cache; 
    }
    
    //  Example request:
    //  .../CourseViewer/GetCoursesByType?courseType=COMP
    [HttpGet("Courses/ByType")]
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
}