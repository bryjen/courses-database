using System.Text.Json;

namespace CoursesDatabase.Config;

/// <summary>
/// Static class containing global variables generally used for database connection.
/// </summary>
public static class DatabaseSettings
{
    public static string DbConnectionString { get; set; }

    static DatabaseSettings()
    {
#if AZURE_DEPLOY
        //  Code during development
        string? dbConnectionString = Environment.GetEnvironmentVariable("COURSES_DATABASE_DB_CONNECTION_STRING", EnvironmentVariableTarget.Machine);
        DbConnectionString = dbConnectionString ?? "";
#else   
        //  Code during development 
        string? filePath = Environment.GetEnvironmentVariable("COURSES_DATABASE_CONFIG_FILEPATH", EnvironmentVariableTarget.Machine);
        string rawJson = File.ReadAllText(filePath ?? "");
        var deserializedData = JsonSerializer.Deserialize<DatabaseSettingsHelper>(rawJson) ?? new DatabaseSettingsHelper();

        DbConnectionString = deserializedData.DbConnectionString;
#endif
    }
}

file class DatabaseSettingsHelper
{
    public string DbConnectionString { get; set; } = null!;
}