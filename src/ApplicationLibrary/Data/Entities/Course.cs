using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Entities;

/// <summary> Represents a specific course/class in a university. </summary>
[Serializable]
[Table("Course")]
public class Course
{
    /// <summary> Internal Id of the course in the database. </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

    public List<string> Prerequisites;

    public Course()
    {
        CourseId = -1;
        UniversityId = -1;
        Type = "NA";
        Number = -1;
        Name = "NA";
        Credits = "NA";
        Description = null;
        Components = null;
        Notes = null;
        Duration = -1;
        Prerequisites = new List<string>();
    }
}
