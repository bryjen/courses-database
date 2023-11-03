using Microsoft.EntityFrameworkCore;
using UniversityDatabase.Configuration;
using UniversityDatabase.Core;

namespace UniversityDatabase.Data.Database;

/// <summary>
///     A <c>DatabaseDataLoader</c> instance represents a session with the specified database instance. The class simply
///     queries the prompted tables; no modifications occur.
/// </summary>
/// <example>
///     <para>
///         Loading data is as simple as instantiating a new instance and getting any of the desired data.
///     </para>
///     <code>
///         var dataLoader = new DatabaseDataLoader();
///         List&lt;Course&gt; courses = dataLoader.Courses;
///         //  ...
///         //  You can now perform operations with the data.
///     </code>
/// </example>
public sealed class DatabaseDataLoader : DbContext
{
    public List<Course> Courses { get; }
    
    private DbSet<Course> CoursesData { get; }
    private DbSet<Course.PrerequisiteCourseData> PrerequisitesData { get; }

    public DatabaseDataLoader() : base()
    {
        //  Read and parse the data from the server
        CoursesData = Set<Course>();
        PrerequisitesData = Set<Course.PrerequisiteCourseData>();
            
        //  Initialize the prerequisites attribute in the Courses objects
        Courses = CoursesData.ToList();
        Courses.ForEach(course => course.InitializePrerequisites(PrerequisitesData.ToList()));
    }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(AppConfig.DbConnectionString);
    }
}