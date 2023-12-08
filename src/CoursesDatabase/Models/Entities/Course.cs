using System.Diagnostics.CodeAnalysis;
using CoursesDatabase.Data.Entities;

namespace CoursesDatabase.Models.Entities;

/// <summary>
/// Class that models a course offered by some university/higher educational institution.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Course
{
    public int UniversityId { get; }
    
    public string Type { get; }
    
    public int Number { get; }
    
    public string Name { get; }
    
    public string Credits { get; }
     
    public string? Description { get; }
    
    public IEnumerable<string>? Components { get; }
    
    public IEnumerable<string>? Notes { get; }
    
    public int Duration { get; }
    
    public IEnumerable<string>? Prerequisites { get; }

    /// <summary>
    /// Creates a new <c>Course</c> object using the passed values.
    /// </summary>
    public Course(int universityId, string type, int number, string name, string credits, string? description,
        IEnumerable<string>? components, IEnumerable<string>? notes, int duration, IEnumerable<string>? prerequisites)
    {
        UniversityId = universityId;
        Type = type.ToLower();
        Number = number;
        Name = name;
        Credits = credits;
        Description = description;
        Components = components;
        Notes = notes;
        Duration = duration;
        Prerequisites = prerequisites;
    }

    /// <summary>
    /// Creates a new <c>Course</c> object. Takes a <c>CourseEntity</c> entity class and a list of prerequisites
    /// courses as string representations. 
    /// </summary>
    /// <remarks>
    /// Generally used when converting from ORM entity classes to domain model classes.
    /// </remarks>
    public Course(CourseEntity courseEntity, IEnumerable<string>? prerequisites)
    {
        UniversityId = courseEntity.UniversityId;
        Type = courseEntity.Type.ToLower();
        Number = courseEntity.Number;
        Name = courseEntity.Name;
        Credits = courseEntity.Credits;
        Description = courseEntity.Description;
        Components = courseEntity.Components;
        Notes = courseEntity.Notes;
        Duration = courseEntity.Duration;
        Prerequisites = prerequisites;
    }

    /// <summary>
    /// The course signature serves as a 'pseudo' unique identifier for a course.
    /// </summary>
    public string CourseSignature => $"{UniversityId} {Type} {Number}".ToLower();

    /// <summary>
    /// Trims out 'less important' data. Greatly reduces the size of list. Remaining data is ideal for 'previewing'
    /// the details of course.
    /// </summary>
    public static IEnumerable<Course> TrimData(IEnumerable<Course> courses)
    { 
        return courses
            .ToList()
            .Select(course => new Course(course.UniversityId, course.Type, course.Number, course.Name, course.Credits, 
                null, null, null, course.Duration, null));
    }

#region ObjectOverrides

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        return this.CourseSignature == ((Course)obj).CourseSignature;
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(CourseSignature);
        return hashCode.ToHashCode();
    }

    public override string ToString() => $"{Type.ToUpper()} {Number}\n{Name} ({Credits} credits)";

#endregion
}