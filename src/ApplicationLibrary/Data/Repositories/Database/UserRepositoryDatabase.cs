using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories.Database;

public class UserRepositoryDatabase : DbContext, IRepository<User>
{
    private readonly string _dbConnectionString;

    private readonly List<User> _users;

    //  DbSet<...> objects
    private DbSet<User> Users { get; set; } = null!;

    public UserRepositoryDatabase(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;

        _users = Users.ToList();
    }
    
    public User? this[int index] => throw new NotImplementedException();

    /// <summary> Get an <c>IEnumerable&lt;User&gt;</c> object containing <b>all</b> data from the data source. </summary>
    public IEnumerable<User> GetAll()
        { return _users; }

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
        new UserTypeConfiguration()
            .Configure(modelBuilder.Entity<User>());
    }
}