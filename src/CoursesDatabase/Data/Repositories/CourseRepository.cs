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
    /// Tries to obtain a specific course from the database.
    /// </summary>
    public Course? TryGetCourse(int courseUniversityId, string courseType, int courseNumber)
    {
        List<CourseEntity> courseEntities = Courses.ToList();
        List<CoursePrerequisiteEntity> coursePrerequisiteEntities = CoursePrerequisites.ToList();
        
        CourseEntity? selectedCourse = courseEntities
            .FirstOrDefault(course => course.UniversityId == courseUniversityId &&
                                      course.Type.Equals(courseType, StringComparison.OrdinalIgnoreCase) &&
                                      course.Number == courseNumber);

        if (selectedCourse is null)
            return null;

        List<CourseEntity> courseAsList = new List<CourseEntity> { selectedCourse }; //  We need to wrap into list to use the conversion method
        return ConvertToModelClasses(courseAsList, courseEntities, coursePrerequisiteEntities).First();
    }

    /// <summary>
    /// Returns a list of courses that have the specified type.
    /// </summary>
    public List<Course> GetCoursesByType(int courseUniversityId, string courseType)
    {
        List<CourseEntity> courseEntities = Courses.ToList();
        List<CoursePrerequisiteEntity> coursePrerequisiteEntities = CoursePrerequisites.ToList();

        List<CourseEntity> selectedCourse = courseEntities
            .Where(course =>  course.UniversityId == courseUniversityId && 
                              course.Type.Equals(courseType, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return (List<Course>) ConvertToModelClasses(selectedCourse, courseEntities, coursePrerequisiteEntities);
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

        return ConvertToModelClasses(courseEntities, courseEntities, coursePrerequisiteEntities);
    }

    /// <summary>
    /// Returns all courses from the database. <b>Course models returned do NOT have their prerequisites initialized.</b>
    /// </summary>
    public IEnumerable<Course> GetAllCoursesUninitialized()
    {
        return Courses
            .Select(courseEntity => new Course(courseEntity, new List<string>()))
            .ToList();
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

    //  Takes in a list of course entity classes 'toConvert', then uses the data from 'courseEntities' and
    // 'coursePrerequisiteEntities' (complete lists of entity classes from DbSet<...> objects) to initialize 
    //  prerequisites data and conversion into model class.
    private static IEnumerable<Course> ConvertToModelClasses(List<CourseEntity> toConvert, List<CourseEntity> courseEntities,
        List<CoursePrerequisiteEntity> coursePrerequisiteEntities)
    {
        List<Course> convertedCourses = new List<Course>();
        
        foreach (CourseEntity courseEntity in toConvert)
        {
            //  List of prerequisite ids.
            //  A sub-list indicates that the corresponding courses are equivalent and can be substituted for one another.
            IEnumerable<IEnumerable<int>> listOfPrerequisiteIds = coursePrerequisiteEntities
                .Where(coursePrereq => coursePrereq.CourseId == courseEntity.CourseId)
                .Select(coursePrereq => coursePrereq.PrereqIds)
                .ToList();

            //  The list of prerequisites but the string representation of their corresponding courses.
            List<string> listOfPrerequisitesAsString = listOfPrerequisiteIds
                .Select(prereqIds => prereqIds.Select(prereqId => courseEntities.First(course => course.CourseId == prereqId)))
                .Select(courses => courses.Select(course => $"{course.Type} {course.Number}"))
                .Select(coursesAsStr => string.Join("/", coursesAsStr))
                .ToList();
            
            convertedCourses.Add(new Course(courseEntity, listOfPrerequisitesAsString));
        }

        return convertedCourses;
    } 
}