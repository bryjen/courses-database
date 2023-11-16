using ApplicationLibrary.Config;

namespace ApplicationLibrary.Tests.Config;

public class AppSettingsTests
{
    [TestCase("courses_database_solution_dir")]
    public void TestSolutionDirectory(string environmentVariable)
    {
        var solutionDirectory = Environment.GetEnvironmentVariable(environmentVariable);

        if (solutionDirectory is null)
        {
            Console.WriteLine("The environment variable \"courses_database_solution_dir\" is not defined.");
            Console.WriteLine("Environment variable just defined for testing purposes, if inconclusive, assure that " + 
                               "\"AppSettings.SolutionDirectory\" returns the solution directory.");
            Assert.Inconclusive();
        }
        
        Console.WriteLine(solutionDirectory);
        Assert.That(AppSettings.SolutionDirectory, Is.EqualTo(solutionDirectory));
    }
}