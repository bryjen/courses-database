using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories.Database;

public class UniversityRepositoryDatabase : DbContext, IRepository<University> 
{
    private readonly string _dbConnectionString;
    
    private readonly List<University> _universities;
    
    private DbSet<University>? Universities { get; set; }
    
    public UniversityRepositoryDatabase(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;

        _universities = Universities!.ToList();
    }
    
    public University? this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IEnumerable<University> GetAll()
    {
        return _universities;
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