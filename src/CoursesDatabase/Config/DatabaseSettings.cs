using System.Text.Json;

namespace CoursesDatabase.Config;

public class DatabaseSettings
{
    public static string DbConnectionString { get; set; }

    static DatabaseSettings()
    {
        string rawJson = File.ReadAllText(@"Config/files/database_settings.json");
        var deserializedData = JsonSerializer.Deserialize<DatabaseSettingsHelper>(rawJson) ?? new DatabaseSettingsHelper();

        DbConnectionString = deserializedData.DbConnectionString;
    }
}

file class DatabaseSettingsHelper
{
    public string DbConnectionString { get; set; } = null!;
}