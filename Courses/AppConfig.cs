namespace Courses;

/// <summary> Class representing a list of program variables that can be changed before runtime. Usually stored and
/// loaded from a config. file in .json/.xml/.yaml. </summary>
[Serializable]
internal sealed class AppConfig
{
    public string DbConnectionString { get; set; } = "";
    public string OutputDirectory { get; set; } = "";
}