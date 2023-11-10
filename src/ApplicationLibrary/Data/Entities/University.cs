using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Entities;

/// <summary>
///     Represents a specific university (or maybe even a college).
/// </summary>
/// <remarks>
///     <para>
///         This class also serves as an entity class (see <see cref="Microsoft.EntityFrameworkCore.DbContext"/>) when
///         loading data from the specified database instance.
///     </para>
/// </remarks>
[Keyless]
[Table("universities")]
public class University
{
    [Column("university-id")]
    public int UniversityId { get; internal set; } = -1;
    
    [Column("formal-name")]
    public string FormalName { get; internal set; } = "NA";
    
    [Column("abbreviation")]
    public string? Abbreviation { get; internal set; } = null;
    
    [Column("location")]
    public string Location { get; internal set; } = "NA";
    
    [Column("type")]
    public string? Type { get; internal set; } = null;
    
    [Column("year-founded")]
    public int? YearFounded { get; internal set; } = null;
    
    [Column("description")]
    public string? Description { get; internal set; } = null;
}