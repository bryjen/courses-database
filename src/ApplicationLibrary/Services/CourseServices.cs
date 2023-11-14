using System.Diagnostics.CodeAnalysis;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories;

namespace ApplicationLibrary.Services;

public class CourseServices
{
    private readonly IRepository<Course> _courseRepository;

    public CourseServices(IRepository<Course> courseRepository)
    {
        this._courseRepository = courseRepository;
    }

    public IEnumerable<Course> SearchCourses(SearchParameters searchParameters)
    {
        return CourseSearcher.SearchCourses(searchParameters, _courseRepository.GetAll());
    }


    /// <summary> Helper class providing course search functionality. </summary>
    private static class CourseSearcher {

        /// <summary> Searches for and returns all courses that satisfy a given set of parameters and a set of courses. </summary>
        public static IEnumerable<Course> SearchCourses(SearchParameters searchParameters, IEnumerable<Course> courses)
        {
            IEnumerable<Course> keywordsFiltered = FilterByKeywords(searchParameters.Keywords, courses);
            IEnumerable<Course> componentsFiltered = FilterByLectureComponents(searchParameters.LectureComponents, courses);
            
            FilterByUniversityId(searchParameters.UniversityId, ref courses);
            FilterByCourseType(searchParameters.CourseType, ref courses);
            FilterByCourseNumber(searchParameters.CourseNumber, ref courses);
            FilterByCourseName(searchParameters.CourseName, ref courses);
            FilterByCredits(searchParameters.Credits, ref courses);
            
            return courses;
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

        private static IEnumerable<Course> FilterByKeywords(IEnumerable<string> keywords, IEnumerable<Course> courses)
        {
            return from course in courses
                from keyword in keywords.Select(keyword => keyword.ToLower().Trim())
                where course.Name.ToLower().Contains(keyword) || course.Description.ToLower().Contains(keyword) || 
                      course.Components.ToLower().Contains(keyword) || course.Notes.ToLower().Contains(keyword)
                select course;
        }

        private static IEnumerable<Course> FilterByLectureComponents(IEnumerable<string> lectureComponents, IEnumerable<Course> courses)
        {
            return from course in courses
                from lecComponent in lectureComponents.Select(component => component.ToLower().Trim())
                where course.Components.ToLower().Contains(lecComponent)
                select course;
        }
    }

    
    /// <summary> Plain class containing parameters affecting the course search. </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class SearchParameters
    {
        public int? UniversityId { get; set; }
        public string? CourseType { get; set; }
        public int? CourseNumber { get; set; }
        public string? CourseName { get; set; }
        public string? Credits { get; set; }
        public IEnumerable<string> Keywords { get; set; }
        public IEnumerable<string> LectureComponents { get; set; }

        public SearchParameters(int? universityId, string? courseType, int? courseNumber, string? courseName,
            string? credits, IEnumerable<string> keywords, IEnumerable<string> lectureComponents)
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
}
