using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;
using ApplicationLibrary.Data.Repositories.Database;
using ApplicationLibrary.Data.Repositories.Serialization;

namespace ApplicationLibrary.Tests.Data.Entities;

[TestFixture]
public class CourseTests
{

    [TestCase(1, "COMP", 228)]
    public void TestTryGetCourseId(int universityId, string courseType, int courseNumber)
    {
        IRepository<Course> courseRepo = new CourseRepositoryDatabase(AppSettings.DbConnectionString);
        List<Course> courses = (List<Course>) courseRepo.GetAll();

        if (!Course.TryGetCourseId(universityId, courseType, courseNumber, courses, out int courseId))
        {
            Assert.Fail();
            return;
        }
        
        Course? selectedCourse = 
           (from course in courses
            where course.UniversityId == universityId && 
                  course.Type == courseType && 
                  course.Number == courseNumber
            select course)
           .FirstOrDefault();

        int expectedCourseId = selectedCourse?.CourseId ?? -1;
        Console.WriteLine($"Expected:\t{expectedCourseId}\nGot:\t\t{courseId}");
        Assert.That(courseId, Is.EqualTo(expectedCourseId));
    }
    
    [TestCase(1, "COMP", 228)]
    [TestCase(1, "COMP", 353)]
    public void TestGetPrerequisitesAsCourseIds(int universityId, string courseType, int courseNumber)
    {
        IRepository<Course> courseSerializer = new CourseRepositoryDeserializer(LocalFileSelector.GetSerializedCoursesFileName() ?? "");
        IRepository<Course> courseDatabase = new CourseRepositoryDatabase(AppSettings.DbConnectionString);
        List<Course> coursesSerialized = (List<Course>) courseSerializer.GetAll();
        List<Course> coursesDatabase = (List<Course>) courseDatabase.GetAll();
        

        Course? selectedCourse = 
           (from course in coursesSerialized
            where course.UniversityId == universityId &&
                  course.Type == courseType &&
                  course.Number == courseNumber
            select course)
           .FirstOrDefault();

        if (selectedCourse is null)
        {
            Assert.Fail("'selectedCourse' is null");
            return;
        }

        var prereqIds = selectedCourse.GetPrerequisitesAsCourseIds(coursesDatabase) ?? new List<List<int>>(); 
        foreach (var prereqIdList in prereqIds.Select(seq => seq.ToList()))
        {
            
            
            prereqIdList.ForEach(id => Console.Write(id + " "));
            Console.WriteLine();
        }
    }
}