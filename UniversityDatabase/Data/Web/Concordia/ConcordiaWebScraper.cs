using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using UniversityDatabase.Core;

namespace UniversityDatabase.Data.Web.Concordia;

/// <summary>
///     An instance of this class represents a web-scraper tailored specifically to extract course data from Concordia's
///     website(s). 
/// </summary>
[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
public sealed class ConcordiaWebScraper : WebScraper
{
    /// <summary>
    ///     Constructs a <c>ConcordiaWebScraper</c> with the urls provided in a .yaml file.
    /// </summary>
    public ConcordiaWebScraper() : base("Data/Web/Concordia/concordia_urls.yaml") { }

    /// <summary>
    ///     Web-scrapes a passed list, and returns a list of <b>RAW</b> string text data from every course identified.
    /// </summary>
    /// <param name="urls"> A list of urls to be web-scraped. </param>
    /// <returns>
    ///     A list of string data. Each string is raw unprocessed data that can be converted to a <c>Course</c> object.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Scraping a particular url may sometimes unpredictably throw an error, hence we allocate
    ///         <see cref="WebScraper.MaximumAttempts"/> attempts to scrape the data in a given url. If it fails all
    ///         the attempts, it is skipped over.
    ///     </para>
    /// </remarks>
    /// <seealso cref="TransformToCourses"/>
    protected override List<string> Scrape(List<string> urls)
    {
        var accumulationList = new List<string>();
        
        foreach (var url in urls)
        {
            var attempts = 0;
            
            webScrapeStart: 
            try
            {
                accumulationList.AddRange(ScrapeWebsite(url));
            }
            catch (Exception exception)     //  Catch any exception
            {
#if DEBUG       //  Print stacktrace when in Debug mode
                Console.WriteLine("Exception caught:");
                Console.WriteLine(exception.StackTrace);
#endif
                //  If web-scraping failed, try again up until a maximum of 'MaximumAttempts' attempts
                if (++attempts < MaximumAttempts)
                    goto webScrapeStart;
            }
            
        }

        return accumulationList;
    }

    /// <summary>
    ///     Scrapes a specific url. Returns a list of <b>RAW</b> string text data from every course identified.
    /// </summary>
    /// <param name="url"> The url of the website to scrape. </param>
    /// <returns>
    ///     A list of string data. Each string is raw unprocessed data that can be converted to a <c>Course</c> object.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Data scraped can be converted into <c>Course</c> object specifically representing a Concordia course.
    ///     </para>
    /// </remarks>
    /// <seealso cref="TransformToCourses"/>
    /// <exception cref="NoSuchElementException"></exception>       //  todo update documentation here
    /// <exception cref="WebDriver.UnpackAndThrowOnError"></exception>
    protected override List<string> ScrapeWebsite(string url)
    {
        //  Initialize a driver for interacting with Chrome (browser)
        using IWebDriver driver = new ChromeDriver();
        
        //  An object that can establish wait conditions on the driver. Default limit/timeout is 10 seconds.
        var  wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
        //  Navigate to webpage
        driver.Navigate().GoToUrl(url);
        
        // Since there is a message where we are prompted to accept cookies, we wait until the specific element is
        // present in the DOM
        var acceptCookiesButton = wait.Until(drv => drv.FindElement(By.Id("cassie_accept_all_pre_banner")));

        // Click the accept cookies button.
        // Most error-prone
        acceptCookiesButton.Click();

        //  Finds all elements that have the class "course"
        var webElements = wait.Until(drv => drv.FindElements(By.ClassName("course")));

        //  TODO: Can be further optimized by splitting work into further threads & then joining
        //  Extracts the raw HTML data of the element as a string
        return webElements.AsParallel()
            .WithMergeOptions(ParallelMergeOptions.AutoBuffered)
            .Select(webElement => webElement.GetAttribute("outerHTML"))
            .ToList();
    }

    public override Course TransformToCourse(string rawString)
    {
        var course = new Course();
        
        Console.WriteLine("\n");
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(rawString);
        

        {   //  Sets the 'title', 'number', 'name', and 'credits' attributes of a course. These attributes should be 
            //  present in ALL courses and CAN NOT be null.
            HtmlNode titleNode = htmlDoc
                .DocumentNode                                                                //  Get root HTML node
                .Descendants("h3")                                                           //  Get all child 'h3' tags
                .First(node => node.GetAttributeValue("class", "").Contains("title"));       //  Filter those who has 'class' equal to 'title'

            var typeRegex = new Regex(@"^\D{4}");
            var numberRegex = new Regex(@"\d{3,5}");
            var creditsRegex = new Regex(@"\(\d(.\d+)? credits?\)");

            //  Attempting to fill in the 'Type' attribute
            var typeMatch = typeRegex.Match(titleNode.InnerText);
            if (typeMatch.Success)
                course.Type = typeMatch.Value;
            else
                throw new ArgumentException(@"Cannot find the value for the 'Type' attribute.\n" +
                                            $"Tag <h3 class=\"title\">;{titleNode.InnerText}");

            //  Attempting to fill in the 'Number' attribute
            var numberMatch = numberRegex.Match(titleNode.InnerText);
            if (numberMatch.Success)
                course.Number = Convert.ToInt32(numberMatch.Value);
            else
                throw new ArgumentException("Cannot find the value for the 'Number' attribute.\n" +
                                            $"Tag <h3 class=\"title\">;{titleNode.InnerText}");

            //  Attempting to fill in the 'Credits' attribute
            var creditsMatch = creditsRegex.Match(titleNode.InnerText);
            if (creditsMatch.Success)
            {
                Match creditsValueMatch = new Regex(@"\.?\d+(.\d+)?").Match(creditsMatch.Value);
                if (creditsValueMatch.Success)
                    course.Credits = creditsValueMatch.Value;
            }
            else
            {
                throw new ArgumentException("Cannot find the value for the 'Credits' attribute.\n" +
                                            $"Tag <h3 class=\"title\">;{titleNode.InnerText}");
            }

            //  Attempting to fill in the 'Name' attribute.
            //  Does so by deleting every other component in the string using regex.
            string name = typeRegex.Replace(titleNode.InnerText, "");
            name = numberRegex.Replace(name, "");
            name = creditsRegex.Replace(name, "");
            name = name.Trim();
            course.Name = name;
        }

        {   //  Sets the 'Description' attribute
            try
            {
                var dataNodes = htmlDoc
                    .DocumentNode                                                                   //  Get root HTML node
                    .SelectSingleNode("//div[@class='content accordion_accordion_panel']")          //  Gets the parent 'div' for the tags containing the data 
                    .SelectNodes("./*")                                                             //  Gets the direct children of the tag
                    .Where(node => !node.Name.Equals("br", StringComparison.OrdinalIgnoreCase));

                Console.WriteLine(course);
                dataNodes.ToList().ForEach(node => Console.WriteLine(Regex.Replace("|>" + node.InnerText, @"[\n]*", "")));
                //dataNodes.ToList().ForEach(node => Console.WriteLine(node));
            } catch (Exception) { } //  Ignored
        }

        



/*
             try
            {
                IEnumerable<HtmlNode> descriptionNode = htmlDoc
                    .DocumentNode
                    .Descendants("p")
                    .Where(node => node.GetAttributeValue("class", "").Contains("crse-descr"));

                var description = descriptionNode
                    .Select(node => node.InnerText)
                    .Aggregate((currentAggregate, text) => currentAggregate + text);

                course.Description = description;
            } catch (Exception) { } //  Ignored
 
        //  Gets 'prerequisites' RAW text
        divs = htmlDoc.DocumentNode
            .Descendants("span")
            .Where(node => node.GetAttributeValue("class", "").Contains("requisites"));
        divs.ToList().ForEach(x => Console.WriteLine(x.InnerText));
        
        try
        {
            //  Gets 'description'
            divs = htmlDoc.DocumentNode
                .Descendants("p")
                .Where(node => node.GetAttributeValue("class", "").Contains("crse-descr"));
            divs.ToList().ForEach(x => Console.WriteLine(x.InnerText));
        } catch (Exception) { } //  Ignored

        try
        {
            //  Gets 'components' RAW text
            divs = htmlDoc.DocumentNode
                .Descendants("span")
                .Where(node => node.GetAttributeValue("class", "").Contains("components"));
            divs.ToList().ForEach(x => Console.WriteLine(x.InnerText));
        } catch (Exception) { } //  Ignored

        try
        {
            //  Gets 'notes' RAW text
            divs = htmlDoc.DocumentNode
                .Descendants("ul")
                .First(node => node.GetAttributeValue("class", "").Contains("course-notes"))
                .Descendants("li")
                .Where(node => node.GetAttributeValue("class", "").Contains("xlarge-text"));
            divs.ToList().ForEach(x => Console.WriteLine("-> " + x.InnerText));
        } catch (Exception) { } //  Ignored
 */

        return new Course();
    }
}