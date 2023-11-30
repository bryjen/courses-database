using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CoursesDatabaseApp.Controllers.Courses;

public class CourseViewerController : Controller
{
    public IActionResult CoursesCatalog()
    {
        using CourseRepository courseRepository = new CourseRepository();
        return View(courseRepository.GetCoursesByType(1, "comp"));
    }
}