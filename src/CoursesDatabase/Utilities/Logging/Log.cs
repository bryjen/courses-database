using Serilog;

namespace CoursesDatabase.Utilities.Logging;

/// <summary>
/// Wrapper class for Serilog's logger functionalities.
/// </summary>
public static class Log
{
    static Log()
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    public static void ToDebug(string message)
    {
        Serilog.Log.Logger.Debug(message);
    }

    public static void ToInformation(string message)
    {
        Serilog.Log.Logger.Information(message);
    }

    public static void ToWarning(string message)
    {
        Serilog.Log.Logger.Warning(message);
    }

    public static void ToError(string message)
    {
        Serilog.Log.Logger.Error(message);
    }

    public static void ToFatal(string message)
    {
        Serilog.Log.Logger.Fatal(message);
    }
}