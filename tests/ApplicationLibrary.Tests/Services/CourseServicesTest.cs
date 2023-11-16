using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;
using ApplicationLibrary.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLibrary.Tests.Services;

[TestFixture]
public class CourseServicesTest
{
    [Description("Tests for basic search functionality on the 'signature' of the course.")]
    [Category("Unit Test")]
    [Retry(3)]
    [TestCase(1, "COMP", null, null, null)]
    [TestCase(1, null, 228, null, null)]
    [TestCase(1, null, null, "machine learning", null)]
    [TestCase(1, null, null, null, "4")]
    [TestCase(1, "ENGR", null, null, "4")]
    [TestCase(1, "AERO", 291, null, "4")]
    [TestCase(1, "COMP", 228, "System Hardware", "3")]
    public void CourseMatchSearch1(int? universityId, string? courseType, int? courseNumber, string? courseName, string? credits)
    {
        var courseRepository = TestSetup.ServiceProvider.GetService<IRepository<Course>>()!;
        var courseService = new CourseServices(courseRepository);
        var searchParameters = new CourseServices.SearchParameters(universityId, courseType, courseNumber, courseName, credits, null, null);
        
        var searchResults = courseService.SearchCourses(searchParameters).ToList();
        var expectedResults =
            from course in courseRepository.GetAll()
            where course.UniversityId == (universityId ?? course.UniversityId)
            where course.Type == (courseType ?? course.Type)
            where course.Number == (courseNumber ?? course.Number)
            where course.Credits == (credits ?? course.Credits)
            select course;
        
        //  Special behavior for querying course name
        if (courseName is not null)
        {
            var tokens = courseName.Trim().ToLower().Split(" ").ToList();
            expectedResults = expectedResults.Where(course => tokens.Any(token => course.Name.ToLower().Contains(token)));
        }
        expectedResults = expectedResults.ToList();
        
        PrintResultsVsExpected(searchResults, expectedResults);
        Assert.That(HasSameElements(searchResults, expectedResults), Is.True);
    }

    [Test]
    public void JustPrint()
    {
        var courseService = new CourseServices(TestSetup.ServiceProvider.GetService<IRepository<Course>>()!);

        var results = courseService.SearchCourses(new CourseServices.SearchParameters(1, "COMP", null, null, null, null, null));
        results.ToList().ForEach(Console.WriteLine);
        
        Assert.Pass();
    }
    
    //  Returns true if two IEnumerable<Course> objects contain the exact same elements (NOT order sensitive)
    private static bool HasSameElements(IEnumerable<Course> first, IEnumerable<Course> second)
    {
        var firstMap = first
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());
        
        var secondMap = second
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());

        return firstMap.Keys.All(x => secondMap.ContainsKey(x) && firstMap[x].Equals(secondMap[x])) &&
               secondMap.Keys.All(x => firstMap.ContainsKey(x) && secondMap[x].Equals(firstMap[x]));
    }

    //  Prints out two IEnumerable<Course> objects. Formatted to show results vs expected.
    private static void PrintResultsVsExpected(IEnumerable<Course> results, IEnumerable<Course> expected)
    {
        Console.WriteLine("SEARCH RESULTS:");
        results.ToList().ForEach(Console.WriteLine);
        
        Console.WriteLine("\nEXPECTED RESULTS:");
        expected.ToList().ForEach(Console.WriteLine);
    }
}
