using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;

namespace ApplicationLibrary.Services;

public class UserServices
{
    
    public UserServices()
    {
    }

    /// <summary>
    /// Attempts to create a user given given an email and password hash. 
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="passwordHash">The password of the user</param>
    /// <param name="logCallback"><c>Serilog</c> function to log information. A <c>null</c> value disables logging.</param>
    /// <returns><c>null</c> if creation is unsuccessful, the newly created <c>User</c> object otherwise.</returns>
    public User? CreateUser(string email, string passwordHash, 
        LogDelegates.WriteDelegate? logCallback = null)
    {
        using UsersManager usersManager = new UsersManager(AppSettings.DbConnectionString);

        if (usersManager.TryCreateUser(email, passwordHash, 
                out User? newUser, 
                out string statusMessage,
                out LogEventLevel messageEventLevel))
        {
            logCallback?.Invoke(messageEventLevel, statusMessage, newUser!.Email);
            return newUser;
        }

        const string messageTemplate = "Failed to create new user with details: \"{email}\", \"{passwordHash}\". {statusMessage}.";
        logCallback?.Invoke(messageEventLevel, messageTemplate, email, passwordHash, statusMessage);
        return null;
    }

    /// <summary>
    /// Determines whether the user with the specified email exists in the database.
    /// </summary>
    /// <param name="email">The email to search for in the database.</param>
    /// <param name="logCallback"><c>Serilog</c> function to log information. A <c>null</c> value disables logging.</param>
    /// <returns>True if the user exists in the database, fale otherwise</returns>
    public bool DoesUserExist(string email,
        LogDelegates.WriteDelegate? logCallback = null)
    {
        logCallback?.Invoke(LogEventLevel.Information, "Attempting to find user with email \"{email}\"...", email);
        
        using UsersManager usersManager = new UsersManager(AppSettings.DbConnectionString);
        User? foundUser = usersManager.GetUser(email);
        
        string messageTemplate = foundUser is null ? 
            "The user with email \"{email}\" DOES NOT exist in the database." : 
            "The user with email \"{email}\" exists in the database.";
        logCallback?.Invoke(LogEventLevel.Information, messageTemplate, email);
        return foundUser is not null;
    } 
    
    /// <summary>
    /// Attempts to get the user with the specified email.
    /// </summary>
    /// <param name="email">The email to search for in the database.</param>
    /// <param name="passwordHash">The hashed password of the user.</param>
    /// <param name="logCallback"><c>Serilog</c> function to log information. A <c>null</c> value disables logging.</param>
    /// <returns><c>null</c> if no user was found, the corresponding <c>User</c> object otherwise.</returns>
    public User? GetUser(string email, string passwordHash,
        LogDelegates.WriteDelegate? logCallback = null)
    {
        logCallback?.Invoke(LogEventLevel.Information, "Attempting to retrieve user with email \"{email}\"...", email);
        
        using UsersManager usersManager = new UsersManager(AppSettings.DbConnectionString);
        User? foundUser = usersManager.GetUser(email, passwordHash);

        string messageTemplate = foundUser is null ? 
            "Could not get user with email \"{email}\"" : 
            "Successfully fetched user with email \"{email}\"";
        logCallback?.Invoke(LogEventLevel.Information, messageTemplate, email);
        return foundUser;
    }

    /// <summary>
    /// Attempts to change the password hash for a given user.
    /// </summary>
    /// <param name="email">The email of the user to change the password hash of.</param>
    /// <param name="newPasswordHash">The new value password hash.</param>
    /// <param name="logCallback"><c>Serilog</c> function to log information. A <c>null</c> value disables logging.</param>
    /// <returns>True if password hash change was successful, false otherwise.</returns>
    public bool UpdateUserPassword(string email, string newPasswordHash,
        LogDelegates.WriteDelegate? logCallback = null)
    {
        logCallback?.Invoke(LogEventLevel.Information, "Attempting to delete the user \"{email}\"...", email);

        using UsersManager usersManager = new UsersManager(AppSettings.DbConnectionString);

        bool isChangeSuccessful = usersManager.UpdatePassword(email, newPasswordHash, 
            out string statusMessage,
            out LogEventLevel messageEventLevel);
        
        logCallback?.Invoke(messageEventLevel, statusMessage);
        return isChangeSuccessful;
    }

    public bool DeleteUser(string email,
        LogDelegates.WriteDelegate? logCallback = null)
    {
        logCallback?.Invoke(LogEventLevel.Information, "Attempting to change the password hash of user \"{email}\"...", email);
        
        using UsersManager usersManager = new UsersManager(AppSettings.DbConnectionString);

        bool isDeleteSuccessful = usersManager.TryDeleteUser(email,
            out User? _,
            out string statusMessage,
            out LogEventLevel messageEventLevel);

        logCallback?.Invoke(messageEventLevel, statusMessage);
        return isDeleteSuccessful;
    }

    public bool VerifyUser(string email,
        LogDelegates.WriteDelegate? logCallback = null)
    {
        return true;
    }
}

/// <summary>
/// Implements lower-level CRUD operations logic on user objects. Operations in this class directly interact with the
/// database through the Entity Framework.
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
    /// Attempts to create a user given an email and a password hash.
    /// </summary>
    /// <param name="newUser">The newly created user. Use if method returns true. </param>
    /// <param name="statusMessage"><c>Serilog</c> message template.</param>
    /// <param name="messageEventLevel"><c>Serilog</c> message level.</param>
    /// <returns> True if creation is successful, false otherwise. </returns>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public bool TryCreateUser(
        string email, string passwordHash, 
        out User? newUser, 
        out string statusMessage, 
        out LogEventLevel messageEventLevel)
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
        {
            statusMessage = $"The email \"{email}\" already exists!";
            messageEventLevel = LogEventLevel.Information;
            return false;
        }
        
        //  Begin transaction
        var addTransaction = Database.BeginTransaction();
        try
        {
            Users.Add(newUser);
            SaveChanges();
            addTransaction.Commit();

            statusMessage = "New user successfully created: {Email}.";
            messageEventLevel = LogEventLevel.Information;
            return true;
        }
        catch
        {
            addTransaction.Rollback();

            statusMessage = "An error occurred while trying to create the user: {email}, {passwordHash}";
            messageEventLevel = LogEventLevel.Error;
            return false;
        }
    }

    /// <summary>
    /// Returns a user with a matching email and password hash. Returns <c>null</c> if none or multiple users have
    /// matching emails and password hashes.
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

    /// <summary>
    /// Returns a user with a matching email. Returns <c>null</c> if none or multiple users have matching emails.
    /// </summary>
    public User? GetUser(string email)
    {
        var users =
           (from user in Users.ToList() 
            where user.Email == email 
            select user)
           .ToList();

        return users.Count == 1 ? users.First() : null;
    }

    
    /// <summary> Attempts to update the password hash of a user. </summary>
    /// <returns> True if change was successful, false otherwise. </returns>
    public bool UpdatePassword(
        string email, string newPasswordHash,
        out string statusMessage,
        out LogEventLevel messageEventLevel)
    {
        var users =
           (from user in Users.ToList() 
            where user.Email == email 
            select user)
           .ToList();

        if (users.Count != 1)
        {
            statusMessage = $"Could not find user with email \"{email}\"";
            messageEventLevel = LogEventLevel.Information;
            return false;
        }

        var updateTransaction = Database.BeginTransaction();
        try
        {
            string oldPasswordHash = users.First().PasswordHash;
            users.First().PasswordHash = newPasswordHash;
            SaveChanges();
            updateTransaction.Commit();

            statusMessage =
                $"Successfully changed the password hash of user \"{email}\" from \"{oldPasswordHash}\" to \"{newPasswordHash}\"";
            messageEventLevel = LogEventLevel.Information;
            return true;
        }
        catch
        {
           updateTransaction.Rollback();
           statusMessage = $"An error occurred while trying to change the password hash for the user \"{email}\"";
           messageEventLevel = LogEventLevel.Error;
           return false;
        }
    }

    /// <summary> Attempts to delete a user from the database given an email. </summary>
    /// <param name="email"> The email of the user to be deleted. </param>
    /// <param name="deletedUser"> The user that was deleted. Null if no user was deleted. </param>
    /// <param name="statusMessage"><c>Serilog</c> message template.</param>
    /// <param name="messageEventLevel"><c>Serilog</c> message level.</param>
    /// <returns> True if deletion was successful, false otherwise. </returns>
    public bool TryDeleteUser(
        string email, 
        out User? deletedUser,
        out string statusMessage,
        out LogEventLevel messageEventLevel)
    {
        deletedUser = null;

        var users = 
           (from user in Users.ToList() 
            where user.Email == email 
            select user)
           .ToList();

        if (users.Count != 1)
        {
            statusMessage = $"Could not find user with email \"{email}\"";
            messageEventLevel = LogEventLevel.Information;
            return false;
        }

        deletedUser = users.First();
        var deleteTransaction = Database.BeginTransaction();
        try
        {
            Users.Remove(deletedUser);
            SaveChanges();
            deleteTransaction.Commit();
            
            statusMessage = $"Successfully deleted the user \"{email}\" from the database.";
            messageEventLevel = LogEventLevel.Information;
            return true;
        }
        catch
        { 
            deletedUser = null; 
            deleteTransaction.Rollback(); 
            statusMessage = $"An error occurred while trying to delete the user \"{email}\"";
            messageEventLevel = LogEventLevel.Error;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_dbConnectionString);
    }

    /// <summary> Generates a token, generally meant for some verification task. </summary>
    private static string GenerateToken()
    {
        var byteArray = RandomNumberGenerator.GetBytes(20);
        return Convert.ToBase64String(byteArray);
    }
}