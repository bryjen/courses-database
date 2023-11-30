using System.Text.Json;

namespace CoursesDatabase.Config;

public static class DatabaseSettings
{
    public static string DbConnectionString { get; set; }

    static DatabaseSettings()
    {
        string filePath = 
            Environment.GetEnvironmentVariable("COURSES_DATABASE_CONFIG_FILEPATH", EnvironmentVariableTarget.Machine) 
            ?? "";
        string rawJson = File.ReadAllText(filePath);
        var deserializedData = JsonSerializer.Deserialize<DatabaseSettingsHelper>(rawJson) ?? new DatabaseSettingsHelper();

        DbConnectionString = deserializedData.DbConnectionString;
    }
}

file class DatabaseSettingsHelper
{
    public string DbConnectionString { get; set; } = null!;
}