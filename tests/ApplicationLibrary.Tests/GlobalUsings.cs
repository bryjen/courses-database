global using NUnit.Framework;
using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;
using ApplicationLibrary.Data.Repositories.Database;
using ApplicationLibrary.Data.Repositories.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLibrary.Tests;

/// <summary>
///     Static helper class containing a <c>Microsoft.Extensions.DependencyInjection.ServiceProvider</c> object (along
///     with other objects) to help manage the test's dependencies and makes DI easier to perform.
/// </summary>
public static class TestSetup
{
    public static readonly ServiceProvider ServiceProvider;

    static TestSetup()
    {
        ServiceProvider = new ServiceCollection()
            .AddSingleton<IRepository<Course>>(_ => new CourseRepositoryDeserializer(LocalFileSelector.GetSerializedCoursesFileName()!))
            .AddSingleton<IRepository<University>>(temp => new UniversityRepositoryDatabase(AppSettings.DbConnectionString))
            .BuildServiceProvider();
    }
}