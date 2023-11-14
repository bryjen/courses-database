using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Database;

/// <summary>
///     A class that can change the contents of the table of a corresponding entity class. 
/// </summary>
/// <typeparam name="T"> An entity/model class in the database. </typeparam>
public abstract class TableManager<T> : DbContext
{
    private readonly string _dbConnectionString;

    protected TableManager(string dbConnectionString)
    {
        this._dbConnectionString = dbConnectionString;
    }

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