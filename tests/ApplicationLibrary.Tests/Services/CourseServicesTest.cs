using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLibrary.Tests.Services;

[TestFixture]
public class CourseServicesTest
{
    private IServiceProvider _serviceProvider = null!;
    
    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .AddScoped<IRepository<Course>, CourseRepositoryDatabase>()
            .BuildServiceProvider();
    }
}