using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursesDatabase.Data.Entities;

/// <summary>
/// Entity class that represents the layout for the <c>App.CoursePrerequisite</c> table in the database. An entity
/// represents a list of equivalent courses that are prerequisites to some other course.
/// </summary>
[Table("CoursePrerequisite", Schema = "App")]
public class CoursePrerequisiteEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LinkId { get; set; }
    
    public int CourseId { get; set; }

    public IEnumerable<int> PrereqIds { get; set; } = null!;
}

/// <summary>
/// Configures the model for the <c>CoursePrerequisiteEntity</c> entity class. 
/// </summary>
internal class CoursePrerequisiteEntityTypeConfiguration : IEntityTypeConfiguration<CoursePrerequisiteEntity>
{
    public void Configure(EntityTypeBuilder<CoursePrerequisiteEntity> builder)
    {
        builder
            .Property(course => course.PrereqIds)
            .HasConversion(
                enumerable => JsonSerializer.Serialize(enumerable, JsonSerializerOptions.Default),
                rawJson => JsonSerializer.Deserialize<IEnumerable<int>>(rawJson, JsonSerializerOptions.Default) 
                           ?? new List<int>());
    }
}