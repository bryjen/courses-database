using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using UniversityDatabase.Core;
using YamlDotNet.Serialization;

namespace UniversityDatabase.Data.Web.Concordia;

public class ConcordiaWebScraper : WebScraper
{
    public void Execute()
    {
        //  Obtaining the list of urls from the 'concordia_urls.yaml' file
        var yaml = File.ReadAllText("Data/Web/Concordia/concordia_urls.yaml");
        var deserializer = new DeserializerBuilder().Build();
        var urls = deserializer.Deserialize<UrlList>(yaml);
        
        foreach (var url in urls.Urls.GetRange(54, 5))
        {
            //  Declaring classes that interact w/ concordia course website
            using IWebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            //  Navigate to webpage
            driver.Navigate().GoToUrl(url);
            
            try
            {
                // Wait until the specific element is present in the DOM
                IWebElement acceptCookiesButton = wait.Until(drv => drv.FindElement(By.Id("cassie_accept_all_pre_banner")));

                // Click the accept cookies button.
                acceptCookiesButton.Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Accept cookies button not found");
            }

            // Wait for a moment to ensure the results are loaded
            var results = wait.Until(drv => drv.FindElements(By.ClassName("course")));
            foreach (var result in results)
            {
                Console.WriteLine(result.Text + "\n");
            }
        }
    }

    public override List<string> ScrapeAll()
    {
        var accumulationList = new List<string>();
        foreach (var url in Urls)
        {
            accumulationList.AddRange(ScrapeWebsite(url));
        }

        return accumulationList;
    }

    protected override List<string> ScrapeWebsite(string url)
    {
        throw new NotImplementedException();
    }

    protected override List<Course> TransformToCourses(List<string> rawStringData)
    {
        throw new NotImplementedException();
    }

    public ConcordiaWebScraper() : base("Data/Web/Concordia/concordia_urls.yaml")
    {
    }
}