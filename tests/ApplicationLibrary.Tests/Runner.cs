using ApplicationLibrary.Services;
using Serilog;

namespace ApplicationLibrary.Tests;

public class Runner
{
    [Test]
    public void Run1()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        
        UserServices userServices = new UserServices();
        Console.WriteLine(userServices.DeleteUser("miguelbryancarlo3434@gmail.com", Log.Write));
    }
}