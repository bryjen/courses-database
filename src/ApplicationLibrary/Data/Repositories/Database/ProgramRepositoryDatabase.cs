using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories.Database;

public class ProgramRepositoryDatabase : DbContext, IRepository<Program>
{
    private readonly string _dbConnectionString;

    private List<Program> _programs;

    //  DbSet<...> objects
    private DbSet<Program> Programs { get; set; } = null!;

    public ProgramRepositoryDatabase(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;

        _programs = Programs.ToList();
    } 
    
    public Program? this[int index] => throw new NotImplementedException();

    public IEnumerable<Program> GetAll()
    {
        throw new NotImplementedException();
    }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }
}