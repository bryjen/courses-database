using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationLibrary.Data.Entities;

/// <summary> This class represents a group of courses. </summary>
[Table("CourseGroup")]
public class CourseGroup
{
    /// <summary> Internal Id of the course group. </summary>
    public int GroupId { get; set; }
    
    /// <summary> Internal Id of the university offering the program. </summary>
    public int UniversityId { get; set; } 
    
    /// <summary> The name of the course group. Ex. <c>Data Science Courses</c></summary>
    public string Name { get; set; } 
    
    /// <summary> The total credits of the course group. </summary>
    public string TotalCredits { get; set; } 
    
    /// <summary> Any extra notes about the course group. </summary>
    public IEnumerable<string>? Notes { get; set; }
    
    public CourseGroup()
    {
        GroupId = -1;
        UniversityId = -1;
        Name = "NA";
        TotalCredits = "NA";
        Notes = null;
    }
}