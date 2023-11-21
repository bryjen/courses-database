using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Services;

public class UserServices
{
    
}

/// <summary>
///     Implements CRUD operations logic on user objects.
/// </summary>
internal class UsersManager : DbContext
{
    private readonly string _dbConnectionString;

    private DbSet<User> Users { get; set; } = null!;

    public UsersManager(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }

    /// <summary>
    ///     Returns a user with a matching email and password hash. Returns <c>null</c> if none or multiple users have
    ///     matching emails and password hashes.
    /// </summary>
    public User? GetUser(string email, string passwordHash)
    {
        var users =
           (from user in Users.ToList() 
            where user.Email == email && 
                  BCrypt.Net.BCrypt.Verify(user.PasswordHash, passwordHash)
            select user)
           .ToList();

        return users.Count == 1 ? users.First() : null;
    }

    /// <summary> Attempts to create a user given an email and a password hash. </summary>
    /// <param name="newUser"> The newly created user. Use if method returns true. </param>
    /// <returns> True if creation is successful, false otherwise. </returns>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public bool TryCreateUser(
        string email, string passwordHash, 
        out User? newUser)
    {
        //  Create new user object
        newUser = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            CreationDate = DateTime.Now,
            LastLoginDate = DateTime.Now,
            EmailVerificationToken = GenerateToken(),
            IsEmailVerified = false
        };
        
        //  Verify that the email is unique
        if (!VerifyEmailUniqueness(email, Users.ToList()))
            return false;
        
        //  Begin transaction
        var addTransaction = Database.BeginTransaction();
        try
        {
            Users.Add(newUser);
            SaveChanges();
            addTransaction.Commit();
            return true;
        }
        catch
        {
            addTransaction.Rollback();
            return false;
        }
    }

    /// <summary> Verifies that the given email is not already registered in the database. </summary>
    /// <returns> True if email is not taken, false otherwise. </returns>
    private static bool VerifyEmailUniqueness(string email, List<User> users)
    {
        var selectedUsers =
           (from user in users
            where user.Email == email
            select user)
           .ToList();

        return selectedUsers.Count == 0;
    }
    
    /// <summary> Generates a token, generally meant for some verification task. </summary>
    private static string GenerateToken()
    {
        var byteArray = RandomNumberGenerator.GetBytes(20);
        return Convert.ToBase64String(byteArray);
    }
}