using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories.Database;

public sealed class CourseRepositoryDatabase : DbContext, IRepository<Course>
{
    private readonly string _dbConnectionString;
        
    private readonly List<Course> _courses;
    
    private DbSet<Course>? Courses { get; set; }

    public CourseRepositoryDatabase(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }
    
    public Course? this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IEnumerable<Course> GetAll()
    {
        return _courses;
    }

    public bool IsValid()
    {
        return Database.CanConnect();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_dbConnectionString);
    }
}