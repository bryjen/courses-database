using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationLibrary.Data.Entities;

[Serializable]
[Table("User", Schema = "Auth")]
public class User
{
    [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    
    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";
    
    public DateTime CreationDate { get; set; }
    
    public DateTime LastLoginDate { get; set; }
    
    public string? PasswordResetToken { get; set; }
    
    public DateTime? PasswordResetTokenExpiryDate { get; set; }
    
    public bool? IsPasswordResetTokenUsed { get; set; }
    
    public string? EmailVerificationToken { get; set; }
    
    public bool IsEmailVerified { get; set; }
}



/// <summary>
///     Implements the logic for defining a model for a <c>DbContext</c> class that wishes to use the class <c>User</c>.
/// </summary>
internal class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        
    }
}