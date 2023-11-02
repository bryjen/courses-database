using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using YamlDotNet.Serialization;


namespace Courses.WebScraping.Concordia;

public class ConcordiaWebScraper : WebScraper
{
    public void Execute()
    {
        //  Obtaining the list of urls from the 'concordia_urls.yaml' file
        var yaml = File.ReadAllText("WebScraping/Concordia/concordia_urls.yaml");
        var deserializer = new DeserializerBuilder().Build();
        var urls = deserializer.Deserialize<UrlList>(yaml);
        
        foreach (var url in urls.Urls.GetRange(54, 1))
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
}