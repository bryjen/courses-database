using System.ComponentModel.DataAnnotations.Schema;
using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace ApplicationLibrary.Data.Repositories.Database;

/// <summary>
///     A class
/// </summary>
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

    /// <summary> Deletes all entities in the table, resetting identity conditions, then adds the passed set of courses. </summary>
    /// <returns> True if transaction was successful, false otherwise. </returns>
    public void ReplaceAllCourses(IEnumerable<Course> newCourses)
    {
        //  Transaction resetting the 'Course' table
        using var resetTransaction = Database.BeginTransaction();
        try
        {
            Database.ExecuteSqlRaw(@"DELETE FROM App.Course");
            Database.ExecuteSqlRaw(@"DBCC CHECKIDENT ('App.Course', RESEED, 0)");
            SaveChanges();
            
            Courses.AddRange(newCourses);
            SaveChanges();
            resetTransaction.Commit();
        }
        catch 
        {
            resetTransaction.Rollback();
        }

        //  Transaction resetting the 'CoursePrerequisite' table
        using var linkingTransaction = Database.BeginTransaction();
        try
        {
            Database.ExecuteSqlRaw(@"DELETE FROM App.CoursePrerequisite");
            SaveChanges();

            List<Course> courses = Courses.ToList();
            
        }
        catch
        {
            linkingTransaction.Rollback();
        }
    } 
    
    //  Sets up the configuration for the db connection.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseSqlServer(_dbConnectionString);
    }

    //  Sets up the model.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        
        modelBuilder.Entity<Course>()
            .Property(course => course.Components)
            .HasConversion( 
                value => JsonConvert.SerializeObject(value, serializerSettings),
                rawJson => JsonConvert.DeserializeObject<List<string>>(rawJson, serializerSettings) ?? new List<string>(),
                new ValueComparer<IEnumerable<string>>(
                    (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()), 
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        
        modelBuilder.Entity<Course>()
            .Property(course => course.Notes)
            .HasConversion( 
                value => JsonConvert.SerializeObject(value, serializerSettings),
                rawJson => JsonConvert.DeserializeObject<List<string>>(rawJson, serializerSettings) ?? new List<string>(),
                new ValueComparer<IEnumerable<string>>(
                    (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()), 
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
                );
        
        
    }
}
