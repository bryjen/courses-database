using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Entities;

/// <summary> Represents a specific course/class in a university. </summary>
[Serializable]
[Table("Course", Schema = "App")]
public class Course
{
    /// <summary> Internal Id of the course in the database. </summary>
    [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CourseId { get; set; }

    /// <summary> Internal Id of the university that offers the course. Used to link with other entities. </summary>
    public int UniversityId { get; set; }
    
    /// <summary> The type of the course. Ex. <c>COMP</c>, <c>ENGR</c> </summary>
    public string Type { get; set; }
    
    /// <summary> The number of the course. Ex. <c>228</c> <c>345</c> </summary>
    public int Number { get; set; }
   
    /// <summary> The name of the course. Ex. <c>System Hardware</c> </summary>
    public string Name { get; set; }
   
    /// <summary> The number of credits offered by the course in string representation. </summary>
    public string Credits { get; set; }
   
    /// <summary> A brief description of the course. </summary>
    public string? Description { get; set; }
   
    /// <summary> The components of the course. Ex. <c>Lecture, Laboratory, ...</c> </summary>
    public IEnumerable<string>? Components { get; set; }
   
    /// <summary> Any other notes about the course. </summary>
    public IEnumerable<string>? Notes { get; set; } 
   
    /// <summary> How many semesters the course is spanned over. </summary>
    public int Duration { get; set; }

    /// <summary> Contains a list of the course prerequisites as strings. </summary>
    /// <remarks> Assume that all listed courses belong to the same university as this course. </remarks>
    [NotMapped]
    public IEnumerable<string>? Prerequisites { get; set; }
    
    [NotMapped]
    [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
    internal IEnumerable<IEnumerable<int>>? PrerequisitesAsCourseIds { get; set; }

    public Course()
    {
        CourseId = 0;       //  Must keep default value of 0, otherwise EF will complain
        UniversityId = -1;
        Type = "NA";
        Number = -1;
        Name = "NA";
        Credits = "NA";
        Description = null;
        Components = null;
        Notes = null;
        Duration = 1;
        Prerequisites = null;
        PrerequisitesAsCourseIds = null;
    }

    /// <summary> Returns the 'signature' of the course with the name and the number of credits. </summary>
    public override string ToString()
        { return $"{Type} {Number} {Name} ({Credits} Credit(s))"; }

    /// <remarks>
    ///     A combination of <c>UniversityId</c>, <c>Type</c>, <c>Number</c>, and <c>Credits</c> are a unique identifier
    ///     to a course. They are only non-readonly because mutability is required for serialization/data-reading.
    /// </remarks>
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        var hashcode = new HashCode();
        hashcode.Add(UniversityId);
        hashcode.Add(Type);
        hashcode.Add(Number);
        hashcode.Add(Credits);
        return hashcode.ToHashCode();
    }

    public IEnumerable<IEnumerable<int>>? GetPrerequisitesAsCourseIds(IEnumerable<Course> courses)
    {
        if (Prerequisites is null)
            return null;

        List<List<int>> listOfCourseIds = new List<List<int>>(); 

        courses = courses.ToList();
        foreach (var prereqAsString in Prerequisites)
        {
            IEnumerable<string> courseSignatures = prereqAsString.Split("/");
            List<int> courseIds = new List<int>();

            Console.WriteLine(prereqAsString);
            
            foreach (var tokens in courseSignatures.Select(str => str.Split(" ")))
            {
                string courseType = tokens[0];
                
                if (!int.TryParse(tokens[1], out int courseNumber))
                    continue;

                if (TryGetCourseId(UniversityId, courseType, courseNumber, courses, out int courseId))
                    courseIds.Add(courseId);
            }
            
            listOfCourseIds.Add(courseIds);
        }
        
        return listOfCourseIds;
    }

    /// <summary>
    ///     Attempts to get the <c>CourseId</c> from a database/list of <c>Courses</c>. Ensure that the list of <c>Course</c>
    ///     objects have been properly initialized. Preferably read it from the database then pass the parsed <c>Course</c>
    ///     objects as a parameter.
    /// </summary>
    /// <returns> True if a single course matches the parameters, false otherwise. </returns>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static bool TryGetCourseId(
        int universityId, 
        string courseType, 
        int courseNumber, 
        IEnumerable<Course> courses, 
        out int courseId)
    {
        var selectedCourse = 
           (from course in courses
            where course.UniversityId == universityId &&
                  course.Type == courseType &&
                  course.Number == courseNumber 
            select course.CourseId)
           .ToList();
        
        courseId = selectedCourse.FirstOrDefault();
        return selectedCourse.Count == 1;
    }
}

[Keyless]
[Table("CoursePrerequisite", Schema = "App")]
internal class CoursePrerequisiteLink
{
   public int CourseId { get; set; }
   public IEnumerable<int> PrereqIds { get; set; } = new List<int>();
}
