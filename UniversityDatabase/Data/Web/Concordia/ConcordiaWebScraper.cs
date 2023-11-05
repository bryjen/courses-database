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

    
    /// <summary>
    ///     Transforms raw web-scraped string data (in HTML format) into a <c>Course</c> object.
    /// </summary>
    /// <param name="rawString"> The raw HTML data as a string. </param>
    /// <returns> A <c>Course</c> object instantiated from the given data. </returns>
    public override Course TransformToCourse(string rawString)
    {
        Course course = new ConcordiaCourse();

        //  Initializes the 'Type', 'Number', 'Credits', and 'Name' attributes
        InitializeBasicData(ref course, rawString);
        
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(rawString);
        
        //  Gets the group of nodes which contains the data for 'Description'
        var dataNodes = htmlDoc
            .DocumentNode                                                                   //  Get root HTML node
            .SelectSingleNode("//div[@class='content accordion_accordion_panel']")          //  Gets the parent 'div' for the tags containing the data 
            .SelectNodes("./*")                                                             //  Gets the direct children of the tag
            .Where(node => !node.Name.Equals("br", StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        //  Initializes 'Description'
        InitializeDescription(ref course, dataNodes);
        
        //  Initializes 'Components'
        InitializeComponents(ref course, dataNodes);
        
        //  Initializes 'Notes'
        InitializeNotes(ref course, dataNodes);
        
        //  Initializes 'PreRequisites'
        InitializePrerequisites(ref course, dataNodes);

#if DEBUG   //  Prints representation of the course when in debug mode
        Console.WriteLine("\n" + course);
        Console.WriteLine("-> " + course.Description);
        Console.WriteLine("-> " + course.Components);
        Console.WriteLine("-> " + course.Notes);
        Console.WriteLine("-> Duration: " + course.Duration);
#endif
        return course;
    }
    

    //  Initializes the 'Type', 'Number', 'Credits', and 'Name' attributes
    //  It does so by accessing the 'h3' node/element with the class of 'title'. 
    //  All the data is stored as a single string in this node and this data is broken down and used for each attribute.
    private static void InitializeBasicData(ref Course course, string rawString)
    {
        //  -------------------------------------Setup the objects------------------------------------------------------
        //  Initialize a 'HtmlDocument' object so we can search for specific tags.
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(rawString);
        
        //  Obtains the node containing the required data
        HtmlNode titleNode = htmlDoc
            .DocumentNode                                                                //  Get root HTML node
            .Descendants("h3")                                                           //  Get all child 'h3' tags
            .First(node => node.GetAttributeValue("class", "").Contains("title"));       //  Filter those who has 'class' equal to 'title'
        
        //  Declare regex objects to parse data
        var typeRegex = new Regex(@"^\D{4}");
        var numberRegex = new Regex(@"\d{3,5}");
        var creditsRegex = new Regex(@"\(\d+(.\d+)? credits?\)");
        
        
        //  -------------------------------------Filling in the values--------------------------------------------------
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

    
    //  Initializes the 'Description' attribute
    //  Takes a list of node/element objects. There exists two cases on how the description is represented:
    //      1. The description is spanned two nodes
    //      2. The description is contained in just one node
    //
    //  First we see if the current node JUST contains the text "Description:"
    //      Then we see if the node after that contains info about the components or the notes. 
    //          IF NOT, then the next data has to be about the description, hence the description is spanned in two
    //          IF YES, error case
    //  So now we know that the description is spanned over two nodes.
    //  We check then if the current node starts with "Description:" then contains some text after
    //          IF YES, we cut of the start, take the rest
    //          IF NO, error case
    private static void InitializeDescription(ref Course course, IReadOnlyList<HtmlNode> dataNodes)
    {
        //  Anonymous function that returns the inner text of an HTML node in lowercase with leading/training spaces removed
        string GetInnerTextSimplified(HtmlNode node) => node.InnerText.ToLower().Trim();
        
        for (var i = 0; i < dataNodes.Count; i++)
        {
            //  Case #1, SEPARATE tags.
            //  If the inner text in the tag is just 'description:' and the next tag does not indicate the 'component'
            //  or 'note' attribute, then the inner text HAS to be for the description attribute
            if (GetInnerTextSimplified(dataNodes[i]) == "description:" &&
                    (!GetInnerTextSimplified(dataNodes[i + 1]).Contains("component(s)")
                    || !GetInnerTextSimplified(dataNodes[i + 1]).Contains("notes")))
            {
                course.Description = dataNodes[i + 1].InnerText.Trim();
                return;
            }
            
            //  Case #2, SAME tag.
            //  Otherwise, if the inner text contains the description, then the value is enclosed within the tag.
            if (new Regex(@"^description:.*").IsMatch(GetInnerTextSimplified(dataNodes[i])))
            {
                course.Description = Regex.Replace(dataNodes[i].InnerText, @"Description:", "").Trim();
                return;
            }
        }

        //  If description is absent, set it as null
        course.Description = null;
    }

    
    //  Initializes the 'Components' attribute
    //  Unlike 'Description', the data for the components attribute occur only in one format. "Component(s): ..."
    //  Hence, we locate the node starting with "Components(s):", then we remove it, resulting in the final data.
    private static void InitializeComponents(ref Course course, IReadOnlyList<HtmlNode> dataNodes)
    {
        //  Attempts to locate the node which starts with the substring "component(s):"
        HtmlNode? node = dataNodes.FirstOrDefault(node => new Regex(@"(?i)component\(s\):?.*").IsMatch(node.InnerText));
        
        //  If not found, set to null. Otherwise, remove the beginning "component(s):" and set the rest as the value
        //  for the attribute
        course.Components = (node is null) ? null : Regex.Replace(node.InnerText, @"(?i)component\(s\):?", "").Trim();
        
        
        //  Data indicating whether or not a course is spanned over two terms is contained HERE, in the 'components'
        if (course.Components is not null && course.Components.ToLower().Contains("two terms"))
            course.Duration = 2;
    }

    
    //  Initializes the 'Notes' attribute
    //  
    private static void InitializeNotes(ref Course course, IReadOnlyList<HtmlNode> dataNodes)
    {
        //  Attempts to locate the node whose class is "course-notes"
        HtmlNode? node = dataNodes
            .FirstOrDefault(node => node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("course-notes"));

        //  If node is null, then there are no notes. Returns
        course.Notes = null;
        if (node is null)
            return;

        //  Select all children nodes, each of which is a separate node. Collapse/join into a single string with ';'
        //  as the delimiter
        var notes = node
            .SelectNodes("./*")
            .Select(htmlNode => Regex.Replace(htmlNode.InnerText, @"[^a-zA-Z0-9' .,-‑]", "").Trim())
            .ToList();
        course.Notes = string.Join(";", notes);
    }


    //  TODO: IMPLEMENT
    private static void InitializePrerequisites(ref Course course, IReadOnlyList<HtmlNode> dataNodes)
    {
        return;
        throw new NotImplementedException();
    }
}