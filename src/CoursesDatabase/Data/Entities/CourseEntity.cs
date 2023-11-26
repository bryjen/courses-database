using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursesDatabase.Data.Entities;

/// <summary>
/// Entity class that represents the layout for the <c>App.Course</c> table in the database. An entity represents some
/// course offered in some university/higher educational institution.
/// </summary>
[Table("Course", Schema = "App")]
public class CourseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CourseId { get; set; }
    
    public int UniversityId { get; set; }
    
    public string Type { get; set; } = null!;
    
    public int Number { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Credits { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public IEnumerable<string>? Components { get; set; }
    
    public IEnumerable<string>? Notes { get; set; }
    
    public int Duration { get; set; }
}

/// <summary>
/// Configures the model for the <c>CourseEntity</c> entity class. 
/// </summary>
internal class CourseEntityTypeConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> builder)
    {
        builder
            .Property(course => course.Components)
            .HasConversion(
                enumerable => JsonSerializer.Serialize(enumerable, JsonSerializerOptions.Default),
                rawJson => JsonSerializer.Deserialize<IEnumerable<string>>(rawJson, JsonSerializerOptions.Default) 
                           ?? new List<string>());
        
        builder
            .Property(course => course.Notes)
            .HasConversion(
                enumerable => JsonSerializer.Serialize(enumerable, JsonSerializerOptions.Default),
                rawJson => JsonSerializer.Deserialize<IEnumerable<string>>(rawJson, JsonSerializerOptions.Default) 
                           ?? new List<string>());
    }
}