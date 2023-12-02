using System.Diagnostics.CodeAnalysis;
using CoursesDatabase.Data.Repositories;
using CoursesDatabase.Models.Entities;

namespace CoursesDatabase.Services.DomainServices;

/// <summary>
/// Static class providing search functionality for more advanced search queries.
/// </summary>
public static class CourseSearchService
{
    /// <summary>
    /// Filters the provided list of courses for those that match the provided course search parameters.
    /// </summary>
    /// <param name="courses"> The list of courses. </param>
    /// <param name="courseSearchParameters"> The parameters of the search/filter. </param>
    public static IEnumerable<Course> FilterCourses(IEnumerable<Course> courses,
        CourseSearchParameters courseSearchParameters)
    {
        return CourseSearcher.SearchCourses(courses, courseSearchParameters);
    }
}

/// <summary>
/// Class containing parameters for course searching
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class CourseSearchParameters
{
    public int? UniversityId { get; set; }
    public string? CourseType { get; set; }
    public int? CourseNumber { get; set; }
    public string? CourseName { get; set; }
    public string? Credits { get; set; }
    public IEnumerable<string>? Keywords { get; set; }
    public IEnumerable<string>? LectureComponents { get; set; }

    public CourseSearchParameters(int? universityId, string? courseType, int? courseNumber, string? courseName,
        string? credits, IEnumerable<string>? keywords, IEnumerable<string>? lectureComponents)
    {
        this.UniversityId = universityId;
        this.CourseType = courseType;
        this.CourseNumber = courseNumber;
        this.CourseName = courseName;
        this.Credits = credits;
        this.Keywords = keywords;
        this.LectureComponents = lectureComponents;
    }
}

/// <summary>
/// Helper class providing course search functionality.
/// </summary>
file static class CourseSearcher
{
    /// <summary> Searches for and returns all courses that satisfy a given set of parameters and a set of courses. </summary>
    public static IEnumerable<Course> SearchCourses(IEnumerable<Course> courses, CourseSearchParameters searchParameters)
    {
        var keywordsFiltered = FilterByKeywords(searchParameters.Keywords, ref courses);
        var componentsFiltered = FilterByLectureComponents(searchParameters.LectureComponents, ref courses);

        if (searchParameters.CourseType is null && searchParameters.CourseNumber is null &&
            searchParameters.CourseName is null && searchParameters.Credits is null)
            return keywordsFiltered.Intersect(componentsFiltered);
            
        FilterByUniversityId(searchParameters.UniversityId, ref courses);
        FilterByCourseType(searchParameters.CourseType, ref courses);
        FilterByCourseNumber(searchParameters.CourseNumber, ref courses);
        FilterByCourseName(searchParameters.CourseName, ref courses);
        FilterByCredits(searchParameters.Credits, ref courses);

        return courses
            .Intersect(keywordsFiltered)
            .Intersect(componentsFiltered);

    }

    //  Filters out courses NOT matching the specified in university id.
    private static void FilterByUniversityId(int? universityId, ref IEnumerable<Course> courses)
    {
        if (universityId is null)
            return;

        courses = from course in courses
            where course.UniversityId == universityId
            select course;
    }

    //  Filters out courses NOT matching the specified in course type.
    private static void FilterByCourseType(string? courseType, ref IEnumerable<Course> courses)
    {
        if (courseType is null)
            return;

        courseType = courseType.ToLower().Trim();   //  trim and convert filter string
        courses = from course in courses
            where course.Type.ToLower().Trim() == courseType
            select course;
    }

    //  Filters out courses NOT matching the specified course number.
    private static void FilterByCourseNumber(int? courseNumber, ref IEnumerable<Course> courses)
    {
        if (courseNumber is null)
            return;

        courses = from course in courses
            where course.Number == courseNumber
            select course;
    }
        
    //  Filters courses whose name matches that of the specified course name.
    private static void FilterByCourseName(string? courseName, ref IEnumerable<Course> courses)
    {
        if (courseName is null)
            return;

        IEnumerable<string> tokens = courseName.Trim().ToLower().Split(" ");
        courses = from course in courses
            from token in tokens
            where course.Name.ToLower().Contains(token)
            select course;
    }

    //  Filters out courses NOT matching the specified number of credits.
    private static void FilterByCredits(string? credits, ref IEnumerable<Course> courses)
    {
        if (credits is null)
            return;

        courses = from course in courses
            where course.Credits == credits
            select course;
    }

    private static IEnumerable<Course> FilterByKeywords(IEnumerable<string>? keywords, ref IEnumerable<Course> courses)
    {
        if (keywords is null)
            return courses;

        keywords = keywords.Select(keyword => keyword.ToLower().Trim());
        return from course in courses
            from keyword in keywords
            where course.Name.ToLower().Contains(keyword) || 
                  (course.Description is not null && course.Description.ToLower().Contains(keyword)) || 
                  (course.Components  is not null && course.Components.Any(component => component.ToLower().Contains(keyword))) || 
                  (course.Notes       is not null && course.Notes.Any(note => note.ToLower().Contains(keyword)))
            select course;
    }

    private static IEnumerable<Course> FilterByLectureComponents(IEnumerable<string>? lectureComponents, ref IEnumerable<Course> courses)
    {
        if (lectureComponents is null)
            return courses;
            
        return from course in courses
            from lecComponent in lectureComponents.Select(component => component.ToLower().Trim())
            where course.Components is not null && course.Components.Any(component => component.ToLower().Contains(lecComponent))
            select course;
    }
}