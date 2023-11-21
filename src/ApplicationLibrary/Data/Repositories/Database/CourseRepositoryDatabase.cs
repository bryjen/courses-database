using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories.Database;

public sealed class CourseRepositoryDatabase : DbContext, IRepository<Course>
{
    private readonly string _dbConnectionString;
        
    private readonly List<Course> _courses;

    //  DbSet<...> objects
    private DbSet<Course> Courses { get; set; } = null!;
    private DbSet<CoursePrerequisiteLink> CoursePrerequisiteLinks { get; set; } = null!;

    public CourseRepositoryDatabase(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;

        _courses = Courses.ToList();
        var coursesCopy = new List<Course>(_courses);
        var prereqs = CoursePrerequisiteLinks.ToList();
        _courses.ForEach(course => course.InitializePrerequisites(coursesCopy, prereqs));
    }
    
    /// <summary> Gets the specified <c>Course</c> at the given index. Returns <c>null</c> if out of bounds. </summary>
    public Course? this[int index] 
        => (index < 0 || index >= _courses.Count) ? null : _courses[index];

    /// <summary> Get an <c>IEnumerable&lt;Course&gt;</c> object containing <b>all</b> data from the data source. </summary>
    public IEnumerable<Course> GetAll()
        { return _courses; }

    /// <summary> Returns whether we can connect to the specified server. </summary>
    public bool IsValid()
        { return Database.CanConnect(); }

    //  Sets up the configuration for the db connection.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
#if DEBUG 
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
#endif
            .UseSqlServer(_dbConnectionString);
    }

    //  Sets up the model.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new CourseEntityTypeConfiguration()
            .Configure(modelBuilder.Entity<Course>());
        
        new CoursePrerequisiteLinkTypeConfiguration()
            .Configure(modelBuilder.Entity<CoursePrerequisiteLink>());
    }
}
