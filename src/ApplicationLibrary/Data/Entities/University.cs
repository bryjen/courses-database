using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Entities;

/// <summary>
///     Represents a specific higher-order educational institution, generally a university, but could be some college.
/// </summary>
[Table("University")]
public class University
{
    /// <summary> Internal Id of the university in the database. </summary>
    public int UniversityId { get; internal set; }
    
    /// <summary> The formal name of the university. </summary>
    public string FormalName { get; internal set; }
    
    /// <summary> The common abbreviation or informal name of the university. </summary>
    public string? Abbreviation { get; internal set; }
    
    /// <summary> The main physical address of the university. </summary>
    public string? Address { get; internal set; }
    
    /// <summary> The type of the university (public, private, etc.). </summary>
    public string Type { get; internal set; }

    /// <summary> The founding year of the university. </summary>
    public int YearFounded { get; internal set; }

    /// <summary> A brief description about the university. </summary>
    public string? Description { get; internal set; }

    public University()
    {
        UniversityId = -1;
        FormalName = "NA";
        Abbreviation = null;
        Address = null;
        Type = "NA";
        YearFounded = -1;
        Description = null;
    }
}