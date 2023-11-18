using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationLibrary.Data.Entities;

/// <summary> Represents a program offered by some higher-level educational institution. </summary>
[Table("UniversityProgram")]
public class Program
{
    /// <summary> Internal Id of the program in the database. </summary>
    public int ProgramId { get; set; }
    
    /// <summary> Internal Id of the university offering the program. </summary>
    public int UniversityId { get; set; }
    
    /// <summary> The name of the program. Ex. <c>BCompSc Joint Major in Data Science</c> </summary>
    public string Name { get; set; } 
    
    /// <summary> The total number of credits required to complete the program. </summary>
    public string TotalCredits { get; set; } 
  
    /// <summary> A brief description on what the program is about. </summary>
    public string? Description { get; set; }    
    
    /// <summary> Any extra notes about the program. </summary>
    public IEnumerable<string>? Notes { get; set; }
   
    /// <summary> Details about the admission requirements. </summary>
    public string? AdmissionRequirements { get; set; }

    public Program()
    {
        ProgramId = -1;
        UniversityId = -1;
        Name = "NA";
        TotalCredits = "NA";
        Description = null;
        Notes = null;
        AdmissionRequirements = null;
    }
}