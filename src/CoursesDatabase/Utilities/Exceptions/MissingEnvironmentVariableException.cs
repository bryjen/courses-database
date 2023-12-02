namespace CoursesDatabase.Utilities.Exceptions;

/// <summary>
/// Exception that is thrown when the project tries to access an environment variable that does not exist.
/// </summary>
public class MissingEnvironmentVariableException : Exception
{
    /// <summary> Name of the environment variable that was attempted to be accessed by the program. </summary>
    public string EnvironmentVariableName { get; }
    
    /// <summary> Target of the environment variable. </summary>
    public EnvironmentVariableTarget Target { get; }

    /// <param name="environmentVariableName"> The name of the environment variable the program tried to access. </param>
    /// <param name="target"> The target search environment for the variable. </param>
    public MissingEnvironmentVariableException(string environmentVariableName, EnvironmentVariableTarget target) :
        base(GenerateMessage(environmentVariableName, target))
    {
        this.EnvironmentVariableName = environmentVariableName;
        this.Target = target;
    }

    /// <summary>
    /// Formats the message of the exception given the target environment and variable name.
    /// </summary>
    private static string GenerateMessage(string environmentVariableName, EnvironmentVariableTarget target)
    {
         return $"\n\nThe program tried to read the environment variable \"{environmentVariableName}\" from environment " +
                $"variable target \"{EnvironmentVariableTargetToString(target)}\". " + 
                $"Ensure that the variable is defined.\n";
    }

    /// <summary>
    /// Returns the string representation of a <see cref="EnvironmentVariableTarget"/> enum.
    /// </summary>
    private static string EnvironmentVariableTargetToString(EnvironmentVariableTarget target)
    {
        return target switch
        {
            EnvironmentVariableTarget.Process => "Process",
            EnvironmentVariableTarget.User =>    "User",
            EnvironmentVariableTarget.Machine => "Machine",
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };
    }
}