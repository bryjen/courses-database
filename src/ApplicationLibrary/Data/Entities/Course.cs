using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    public List<string> Prerequisites { get; set; }
    
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
        Prerequisites = new List<string>();
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

    /// <summary> Initializes the <c>Prerequisites</c> attribute given data loaded from a database. </summary>
    /// <remarks> <ul><li>Called when loading data from a database. Since a course and the list of prerequisites are
    ///                   separated between two tables, those two tables must be passed in.</li></ul> </remarks>
    public void InitializePrerequisites(
        IEnumerable<Course> courses, 
        IEnumerable<CoursePrerequisiteLink> coursePrerequisiteLinks)
    {
        var requiredLinks =
           (from link in coursePrerequisiteLinks
            where link.CourseId == this.CourseId
            select link)
           .ToList();

        var coursesAsList = courses.ToList();

        foreach (var link in requiredLinks)
        {
            List<string> courseSignatures = new List<string>();

            foreach (var courseId in link.PrereqIds)
                if (TryGetCourseSignature(courseId, coursesAsList, out string courseSignature))
                    courseSignatures.Add(courseSignature);

            Prerequisites.Add(string.Join("/", courseSignatures));
        }
    } 

    /// <summary>
    ///     Attempts to get the <c>CourseId</c> from a database/list of <c>Courses</c>. Ensure that the list of <c>Course</c>
    ///     objects have been properly initialized. Preferably read it from the database then pass the parsed <c>Course</c>
    ///     objects as a parameter.
    /// </summary>
    /// <returns> True if a single course matches the parameters, false otherwise. </returns>
    public static bool TryGetCourseId(
        int universityId, string courseType, int courseNumber, 
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

    /// <summary>
    ///     Attempts to get the course signature of a specific course from a database/list of <c>Course</c> objects.
    ///     Ensure that the list of <c>Course</c> objects have been properly initialized. Preferably read it from the
    ///     database and then pass the parsed <c>Course</c> objects as parameter.
    /// </summary>
    /// <returns> True if a single course matches the parameters, false otherwise. </returns>
    private static bool TryGetCourseSignature(
        int courseId,
        IEnumerable<Course> courses,
        out string courseSignature)
    {
        var selectedCourse =
            (from course in courses
                where course.CourseId == courseId
                select course)
            .ToList();

        var firstCourse = selectedCourse.FirstOrDefault();
        courseSignature = (firstCourse is null ? "" : $"{firstCourse.Type} {firstCourse.Number}");
        return selectedCourse.Count == 1;
    }
}



/// <summary> Represents a link between a course, and their prerequisites. </summary>
[Table("CoursePrerequisite", Schema = "App")]
public class CoursePrerequisiteLink
{
    /// <summary> Internal Id of the link entry. </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LinkId { get; set; }
    
    /// <summary> Id of the course the following prerequisite data. </summary>
    public int CourseId { get; set; }
    
    /// <summary> Serialized JSON array of course Ids corresponding to interchangeable course prerequisites. </summary>
    public IEnumerable<int> PrereqIds { get; set; } = new List<int>();
    
    public CoursePrerequisiteLink() 
    { }

    public CoursePrerequisiteLink(int courseId, IEnumerable<int> prereqIds)
    {
        CourseId = courseId;
        PrereqIds = prereqIds;
    }
}



/// <summary> Class containing generic serializing/de-serializing functions. </summary>
file static class Serializers
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static Expression<Func<IEnumerable<T>?, string>> IEnumerableSerializer<T>() => 
        (enumerable => JsonSerializer.Serialize(enumerable ?? Enumerable.Empty<T>(), SerializerOptions));
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static Expression<Func<string, IEnumerable<T>?>> IEnumerableDeserializer<T>() =>
        (rawJson => JsonSerializer.Deserialize<IEnumerable<T>>(rawJson, SerializerOptions) ?? new List<T>());
}



/// <summary>
///     Implements the logic for defining the model for a <c>DbContext</c> class that wishes use the class <c>Course</c>.
/// </summary>
internal class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder
            .Property(course => course.Components)
            .HasConversion(
                Serializers.IEnumerableSerializer<string>(),
                Serializers.IEnumerableDeserializer<string>());
        
        builder
            .Property(course => course.Notes)
            .HasConversion(
                Serializers.IEnumerableSerializer<string>(),
                Serializers.IEnumerableDeserializer<string>());
    }
}



/// <summary>
///     Implements the logic for defining a model for a <c>DbContext</c> class that wishes to use the class
///     <c>CoursePrerequisiteLink</c>.
/// </summary>
internal class CoursePrerequisiteLinkTypeConfiguration : IEntityTypeConfiguration<CoursePrerequisiteLink>
{
    public void Configure(EntityTypeBuilder<CoursePrerequisiteLink> builder)
    {
        //  TODO : Examine effects of warning.
        builder
            .Property(link => link.PrereqIds)
            .HasConversion(
                Serializers.IEnumerableSerializer<int>()!,
                Serializers.IEnumerableDeserializer<int>()!);
    }
}