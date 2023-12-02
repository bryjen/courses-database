using System.Text.Json;
using CoursesDatabase.Utilities.Exceptions;

namespace CoursesDatabase.Config;

/// <summary>
/// Static class containing global variables generally used for database connection. Uses environment variables.
/// </summary>
public static class DatabaseSettings
{
    //  The name of the system environment variable containing the database connection string
    private const string DbConnectionStringEnvVariable = "COURSES_DATABASE_DB_CONNECTION_STRING";

    private const EnvironmentVariableTarget EnvironmentVariableTarget = System.EnvironmentVariableTarget.Machine;
   
    /// <summary> Connection string to the provided database. </summary>
    public static string DbConnectionString { get; set; }

    static DatabaseSettings()
    {
        string? dbConnectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVariable, EnvironmentVariableTarget);

        DbConnectionString = dbConnectionString 
                             ?? throw new MissingEnvironmentVariableException(DbConnectionStringEnvVariable, EnvironmentVariableTarget);
    }
}