using YamlDotNet.Serialization;

namespace UniversityDatabase.Configuration;

/// <summary>
///     A static class that contains configuration data for the program.
/// </summary>
/// <example>
///     <para>
///         Values can be accessed statically in any context anywhere in the assembly as seen below:
///     </para>
///     <code>
///         //  ... Some function/method
///         var connectionString = AppConfig.DbConnectionString;
///         var outputDirectory = AppConfig.OutputDirectory;
///         var apiKey = AppConfig.OpenAiApiKey;
///     </code>
/// </example>
internal static class AppConfig
{
    public static readonly string DbConnectionString;
    public static readonly string OutputDirectory;
    public static readonly string OpenAiApiKey;

    static AppConfig()
    {
        //  Obtaining data from the specified file
        string rawYaml = File.ReadAllText("config.yaml");
        var deserializer = new DeserializerBuilder().Build();
        var deserializedData = deserializer.Deserialize<_AppConfig>(rawYaml);
        
        //  Setting the static attributes
        DbConnectionString = deserializedData.DbConnectionString;
        OutputDirectory = deserializedData.OutputDirectory;
        OpenAiApiKey = deserializedData.OpenAiApiKey;
    }

    
    
    /// <summary>
    ///     A plain C# class containing the configuration data for the program. Values are meant to be matched from
    ///     another source (ex. deserialization from a config file).
    /// </summary>
    [Serializable]
    internal sealed class _AppConfig
    {
        public string DbConnectionString { get; set; } = "";
        public string OutputDirectory { get; set; } = "";
        public string OpenAiApiKey { get; set; } = "";
    }
}