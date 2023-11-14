using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Entities;

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
[Keyless]
[Table("courses")]
public class Course
{
    //  Backing fields
    private string? _description;
    private string? _components;
    private string? _instructors;
    private string? _notes;
    private string? _termsOffered;

    /// <summary> Id of the university in which the course belongs to. </summary>
    [Column("university-id")]
    public int UniversityId { get; internal set; }
    
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
        internal set => _description = value;
    }

    /// <summary> The corresponding components of the course. ex. Lecture/Lab/Tutorial ... </summary>
    [Column("components")]
    public string? Components
    {
        get =>          _components ?? "NA";
        internal set => _components = value;
    }

    /// <summary> The main instructor/professor of the course. </summary>
    [Column("instructors")]
    public string? Instructors
    {
        get =>          _instructors ?? "NA";
        internal set => _instructors = value;
    }

    /// <summary> Any other notable information about the course. </summary>
    [Column("notes")]
    public string? Notes
    {
        get =>          _notes ?? "NA";
        internal set => _notes = value!;
    }

    /// <summary>
    ///     The terms where the course can be taken/is offered. A 'null' value indicates it can be taken in any term.
    /// </summary>
    [Column("terms-offered")]
    public string? TermsOffered
    {
        get =>          _termsOffered ?? "NA";
        internal set => _termsOffered = value;
    }

    /// <summary> How many semesters the course is spread over. </summary>
    [Column("duration")]
    public int Duration { get; internal set; } = 1; 

    /// <summary> A list of prerequisite courses in string format. </summary>
    public List<string> Prerequisites { get; set; } = new List<string>();

    public PrerequisiteCourseData GetPrerequisiteCourseData()
    { 
        return new PrerequisiteCourseData(UniversityId, Type, Number, string.Join("~", Prerequisites));
    }
    
    /// <summary> Initializes the <see cref="Prerequisites"/> attribute. </summary>
    /// <param name="prerequisitesData">
    ///     A list of <c>PrerequisiteCourseData</c> objects containing all courses and their prerequisites.
    /// </param>
    /// <remarks>
    ///     This method is meant to be called directly after initializing a <c>Course</c> object, before it can be used
    ///     in any other statement or in 'client' code.
    /// </remarks>
    internal void InitializePrerequisites(IEnumerable<PrerequisiteCourseData> prerequisitesData)
    {
    }

    /// <summary> Returns a string representation of the <code>Course</code> object. </summary>
    public override string ToString()
    {
        return $"{Type}|{Number}|{Name}|{Credits}";
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
    ///         database instance.
    ///     </para>
    /// </remarks>
    [Table("courses-prerequisites")]
    public sealed class PrerequisiteCourseData
    {
        public PrerequisiteCourseData(int universityId, string type, int number, string prerequisitesString)
        {
            UniversityId = universityId;
            Type = type;
            Number = number;
            PrerequisitesString = prerequisitesString;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("entry-id")]
        public int Id { get; set; }

        [Column("university-id")]
        public int UniversityId { get; internal set; }
        
        [Column("type")]
        public string Type { get; internal set; }

        [Column("number")]
        public int Number { get; internal set; }

        [Column("prereqs")]
        public string PrerequisitesString { get; internal set; }

        public override string ToString()
        {
            return $"{UniversityId} | {Type} | {Number} | {PrerequisitesString}";
        }

        /// <summary> Splits the object into two or more <code>PrerequisiteCourseData</code> objects if they have more
        ///           than one prerequisite. </summary>
        /// <example> "COMP 248~MATH 203~MATH 204" -> "COMP 248" "MATH 203" "MATH 204" on 3 separate objects. </example>
        public IEnumerable<PrerequisiteCourseData> SplitPrerequisiteCourseData()
        {
            IEnumerable<string> splitPrerequisitesString = PrerequisitesString.Split("~");
            return splitPrerequisitesString.Select(str => new PrerequisiteCourseData(UniversityId, Type, Number, str));
        }
    }
}
