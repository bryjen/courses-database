using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories.Database;

public class CourseGroupRepositoryDatabase : DbContext, IRepository<CourseGroup>
{
    private readonly string _dbConnectionString;

    private readonly List<CourseGroup> _courseGroups;

    //  DbSet<...> objects
    private DbSet<CourseGroup> CourseGroups { get; set; } = null!;

    public CourseGroupRepositoryDatabase(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;

        _courseGroups = CourseGroups.ToList();
    }
    
    
    public CourseGroup? this[int index] 
        => (index < 0 || index >= _courseGroups.Count) ? null : _courseGroups[index];

    public IEnumerable<CourseGroup> GetAll()
    {
        throw new NotImplementedException();
    }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }
}