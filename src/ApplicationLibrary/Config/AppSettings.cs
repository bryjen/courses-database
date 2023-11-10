using System.Text.Json;

namespace ApplicationLibrary.Config;

public static class AppSettings
{
    public static readonly string DbConnectionString;

    static AppSettings()
    {
        //  Obtaining data from the specified file
        string rawJson = File.ReadAllText("Config/files/appsettings.json");
        var deserializedData = JsonSerializer.Deserialize<_AppSettings>(rawJson);
        
        //  Setting the static attributes
        DbConnectionString = deserializedData!.DbConnectionString;
    }
    
    /// <summary>
    ///     A plain C# class containing the configuration data for the program. Values are meant to be matched from
    ///     another source (ex. deserialization from a config file).
    /// </summary>
    [Serializable]
    internal sealed class _AppSettings
    {
        public string DbConnectionString { get; set; } = "";
    }
}