using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Database;


/// <summary>
///     This class aims to provide an interface to more easily manipulate entries in a table of the given database.
/// </summary>
public abstract class TableManager<T> : DbContext
{
    private readonly string _dbConnectionString;

    protected TableManager(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }

    /// <summary> Adds an object to to the specified table. </summary>
    public abstract void Add(T entry);

    /// <summary> Attempts to remove an entry from the database that matches the passed object. </summary>
    public abstract bool Remove(T entry);

    /// <summary> Drops all entries in the table. </summary>
    public abstract void DropAll();
    
    /// <summary> Replaces all entries in the table with the specified list of new entries. </summary>
    public abstract void ReplaceAllWith(IEnumerable<T> newEntries);

    /// <summary> Gets all entries in the specified table. </summary>
    /// <remarks> Queries the database every function call. Could be costly. </remarks>
    public abstract IEnumerable<T> GetAllEntities();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_dbConnectionString);
    }
}