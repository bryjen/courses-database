using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using UniversityDatabase.Data.Database;

namespace UniversityDatabase.Core;

/// <summary>
///     Represents a specific course/class in a university.
/// </summary>
/// <remarks>
///     <para>
///         This class also serves as an entity class (see <see cref="Microsoft.EntityFrameworkCore.DbContext"/>) when
///         loading data from the specified database instance.
///     </para>
/// </remarks>
/// <seealso cref="PrerequisiteCourseData"/>
/// <seealso cref="DatabaseDataLoader"/>>
[Keyless]
[Table("courses")]
public class Course
{
    private string? _description = "NA";
    
    private string? _components = "NA";
    
    private string? _notes = "NA";

    /// <summary> The type/department of the course. </summary>
    [Column("type")]
    public string Type { get; internal set; } = "NA";
    
    [Column("number")]
    public int Number { get; internal set; }
    
    [Column("name")]
    public string Name { get; internal set; } = "NA";
    
    [Column("credits")]
    public string Credits { get; internal set; } = "NA";
    
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
    
    /// <summary> Initializes the <see cref="Prerequisites"/> attribute. </summary>
    /// <param name="prerequisitesData">
    ///     A list of <c>PrerequisiteCourseData</c> objects containing all courses and their prerequisites.
    /// </param>
    /// <remarks>
    ///     <para>
    ///         This method is meant to be called directly after initializing a <c>Course</c> object, before it can be
    ///         used in any other statement or in 'client' code.
    ///     </para>
    /// </remarks>
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

    /// <summary> Returns a string representation of the <code>Course</code> object. </summary>
    public override string ToString()
    {
        return $"{Type}|{Number}|{Name}|{Credits}";
    }
    
    /// <summary> Returns a list of <c>Course</c> objects from a database instance. </summary>
    /// <remarks>
    ///     Connects to the database instance when called, which could be time consuming. Use as sparingly as possible.
    /// </remarks>
    /// <returns> A list of <c>Course</c> objects from a database instance. </returns>
    public static List<Course> GetCourses()
    {
        var dataLoader = new DatabaseDataLoader();
        return dataLoader.Courses;
    }
    
    
    
    /// <summary>
    ///     A helper class that stores data about which course is a prerequisite to another course.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Prerequisite data is stored in another table in the database. As such, another entity class is required
    ///         when querying data from the database instance.
    ///     </para>
    ///     <para>
    ///         This class is required solely to instantiate the <see cref="Course.Prerequisites"/> attribute in
    ///         <see cref="Course"/>. As such, it is not meant to be used anywhere else in the assembly and its lifetime
    ///         is intended to be until the 'full initialization' of <c>Course</c> objects when loaded from the
    ///         database instance. To see more, see <see cref="DatabaseDataLoader"/>.
    ///     </para>
    /// </remarks>
    [Keyless]
    [Table("courses-prerequisites")]
    internal sealed class PrerequisiteCourseData
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
}

