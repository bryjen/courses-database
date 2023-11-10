using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;

namespace ApplicationLibrary.Tests.Data.Repositories;

[TestFixture]
public class UniversityRepositoryDatabaseTests
{
    [Test, Timeout(30_000), Order(1)]
    [Description("Tests if we can connect to the specified database within the timeout time.")]
    public void ConnectionTest()
    {
        IRepository<University> universityRepo = new UniversityRepositoryDatabase(AppSettings.DbConnectionString);

        if (universityRepo.IsValid())
            Assert.Pass();
        else
            Assert.Fail();
    }
    
    [Test, Timeout(30_000), Order(2)]
    [Description("Prints out the 'University' objects obtained from the database.")]
    public void PrintAll()
    {
        IRepository<University> universityRepo = new UniversityRepositoryDatabase(AppSettings.DbConnectionString);
        universityRepo.GetAll().ToList().ForEach(university => Console.WriteLine($"{university.FormalName}\n"));
        Assert.Pass();
    }
}