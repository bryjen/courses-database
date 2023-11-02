using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Courses;

[Keyless]
[Table("courses")]
public class Course
{
    //  Backing fields
    private string? _description = "NA";
    private string? _components = "NA";
    private string? _notes = "NA";

    /// <summary> The type/department of the course. </summary>
    [Column("type")]
    public string Type { get; internal set; } = "NA";
    
    /// <summary> The number of the course. </summary>
    [Column("number")]
    public int Number { get; internal set; }

    /// <summary> The name of the course. </summary>
    [Column("name")]
    public string Name { get; internal set; } = "NA";

    /// <summary> The number of credits the course is worth. </summary>
    [Column("credits")]
    public string Credits { get; internal set; } = "NA";
    
    /// <summary> A brief description of the course. </summary>
    [Column("description")]
    public string? Description
    {
        get =>          _description ?? "NA";
        internal set => _description = value!;
    }

    /// <summary> The corresponding components of the course. ex. Lecture/Lab/Tutorial ... </summary>
    [Column("components")]
    public string? Components
    {
        get =>          _components ?? "NA";
        internal set => _components = value!;
    }

    /// <summary> Any other notable information about the course. </summary>
    [Column("notes")]
    public string? Notes
    {
        get =>          _notes ?? "NA";
        internal set => _notes = value!;
    }
    
    /// <summary> How many semesters the course is spread over. </summary>
    [Column("duration")]
    public int Duration { get; internal set; }

    /// <summary> A list of prerequisite courses in string format. </summary>
    // ReSharper disable once MemberCanBePrivate.Global, CollectionNeverQueried.Global
    public List<string> Prerequisites { get; } = new List<string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prerequisitesData"></param>
    internal void InitializePrerequisites(IEnumerable<PrerequisiteCourseData> prerequisitesData)
    {
        //  LINQ Query Syntax to find all prerequisite courses (signature only) for this course
        var listOfPrerequisites = 
            from data in prerequisitesData 
            where data.CourseType == Type && data.CourseNumber == Number
            orderby data.PrerequisiteCourseType
            select (data.PrerequisiteCourseType, data.PrerequisiteCourseNumber);
        
        //  Append signatures to list of prerequisites
        foreach (var dataTuple in listOfPrerequisites)
        {
            Prerequisites.Add($"{dataTuple.PrerequisiteCourseType} {dataTuple.PrerequisiteCourseNumber}");
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public string GetSignature()
    {
        return $"{Type} {Number}";
    }

    /// <summary> Returns a string representation of the <code>Course</code> object. </summary>
    public override string ToString()
    {
        return $"{Type}|{Number}|{Name}|{Credits}";
    }
}

[Keyless]
[Table("courses-prerequisites")]
internal class PrerequisiteCourseData
{
    [Column("prereq-type")]
    public string PrerequisiteCourseType { get; internal set; } = "NA";
    
    [Column("prereq-number")]
    public int PrerequisiteCourseNumber { get; internal set; }

    [Column("type")] 
    public string CourseType { get; internal set; } = "NA";
    
    [Column("number")]
    public int CourseNumber { get; internal set; }

    public override string ToString()
    {
        return $"[prereq] {PrerequisiteCourseType} {PrerequisiteCourseNumber} -> [actual] {CourseType} {CourseNumber}";
    }
}