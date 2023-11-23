using System.Text.Json;
using ApplicationLibrary.Config;
using ApplicationLibrary.Data.Entities;
using ApplicationLibrary.Data.Repositories.Database;
using ApplicationLibrary.Data.WebScraping;
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
        userServices.DoesUserExist("miguelbryancarlo3434@gmail.com", Log.Write);
    }
}