using CoursesDatabase.Config;
using CoursesDatabase.Data.Entities;
using CoursesDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoursesDatabase.Data.Repositories;

/// <summary>
/// Repository that performs low level CRUD functionality with course and course prerequisite data. Additionally
/// provides functionality for obtaining application model classes instead of entity/ORM classes.
/// </summary>
public class CourseRepository : DbContext
{
    private readonly string _dbConnectionString;

    private DbSet<CourseEntity> Courses { get; set; } = null!;
    private DbSet<CoursePrerequisiteEntity> CoursePrerequisites { get; set; } = null!;

    public CourseRepository()
    {
        _dbConnectionString = DatabaseSettings.DbConnectionString;
    }

    /// <summary>
    /// Returns all courses from the database.
    /// </summary>
    /// <remarks>
    /// Returns <b>model</b> classes, not data entity classes.
    /// </remarks>
    public IEnumerable<Course> GetAllCourses()
    {
        List<CourseEntity> courseEntities = Courses.ToList();
        List<CoursePrerequisiteEntity> coursePrerequisiteEntities = CoursePrerequisites.ToList();
        List<Course> courseModels = new List<Course>();
        
        foreach (CourseEntity courseEntity in courseEntities)
        {
            IEnumerable<IEnumerable<int>> listOfPrerequisiteIds = coursePrerequisiteEntities
                .Where(coursePrereq => coursePrereq.CourseId == courseEntity.CourseId)
                .Select(coursePrereq => coursePrereq.PrereqIds)
                .ToList();

             List<string> listOfPrerequisiteStrings = listOfPrerequisiteIds
                .Select(prereqIds => courseEntities.Where(course => prereqIds.Contains(course.CourseId)))
                .Select(courses => courses.Select(course => $"{course.Type} {course.Number}"))
                .Select(coursesAsStrings => string.Join("/", coursesAsStrings))
                .ToList();
             
             courseModels.Add(new Course(courseEntity, listOfPrerequisiteStrings));
        }

        return courseModels;
    }

    //  Configures connection
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableDetailedErrors()
            .UseSqlServer(_dbConnectionString);
    }

    //  Builds entity models
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new CourseEntityTypeConfiguration()
            .Configure(modelBuilder.Entity<CourseEntity>());
        
        new CoursePrerequisiteEntityTypeConfiguration()
            .Configure(modelBuilder.Entity<CoursePrerequisiteEntity>());
    }
}