using Serilog.Events;

namespace ApplicationLibrary.Logging;

/// <summary>
///     
/// </summary>
public abstract class LogDelegates
{
    public delegate void WriteDelegate(LogEventLevel logEventLevel, string messageTemplate, params object[] propertyValues);
}