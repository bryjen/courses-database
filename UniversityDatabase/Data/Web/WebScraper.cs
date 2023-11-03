using UniversityDatabase.Core;
using YamlDotNet.Serialization;

namespace UniversityDatabase.Data.Web;

/// <summary>
///     <para>
///         An instance of this class represents an object that web-scrapes a university's website(s) for data about the
///         courses/classes offered (<b>NOT PROGRAMS</b>). Since each university has their own way to represent a course
///         (one university may list the professor teaching the course, while another university may not), we have to
///         implement university-specific behavior for parsing course data.
///     </para>
///     <para>
///         As such, this class is meant to be extended. Subclasses must define their own behavior for scraping, and
///         parsing data into <c>Course</c> objects.
///     </para>
///     <para>
///         A subclass must call the base constructor <see cref="WebScraper(string)"/>  specifying the filepath to a
///         <c>.yaml</c> file containing a list of urls to be deserialized. This file must be able to be deserialized
///         into a <c>UrlList</c> object (see <see cref="UrlList"/>). After that, the subclass must override the
///         abstract methods to define custom/tailored behavior. See the example code below:
///     </para>
/// </summary>
/// <example>
///     <code>
///         public class HarvardWebScraper : WebScraper
///         {
///             public HarvardWebScraper() : base("harvard_urls.yaml")  //  .yaml contains urls (see below)
///             {
///                 //  Initialize attributes
///                 //  Other behavior
///                 //  ...
///             }
///
/// 
///             public override List&lt;string&gt; ScrapeAll()
///             {
///                 var accumulationList = new List&lt;string&gt;();
///                 foreach (var url in Urls)
///                 {
///                     accumulationList.AddRange(ScrapeWebsite(url));
///                     //  Additional behavior
///                     //  ...
///                 }
///
///                 //  Additional behavior
///                 //  ...
///                 return accumulationList;
///             }
/// 
///
///             protected override List&lt;string&gt; ScrapeWebsite(string url)
///             {
///                 //  Use Selenium, for example, to:
///                 //  1. Load a website
///                 //  2. Search for all elements with a specific id
///                 //  3. Organize all raw text from elements into a List&lt;string&gt;
///                 //  4. Return list
///                 //  ...
///
///                 //  You can use other libraries like HtmlAgilityPack, AngleSharp, etc.
///             }
///
/// 
///             protected override List&lt;string&gt; TransformToCourses(List&lt;string&gt; rawStringData)
///             {
///                 //  Examine each string, establish a common pattern, and parse them into a Course object using
///                 //  your own logic.
///             }
///         }
///     </code>
/// </example>
public abstract class WebScraper
{
    /// <summary> The list of urls to be web-scraped. </summary>
    protected List<string> Urls { get; private set; }

    /// <summary>
    ///     Parameterized constructor that takes a file path to a .yaml file containing the list of urls to be web-
    ///     scraped.
    /// </summary>
    /// <param name="filePath">
    ///     The filepath of the .yaml file containing the (serialized) list of urls.
    /// </param>
    /// <remarks>
    ///     <para>
    ///         The .yaml MUST be able to be deserialized into a <c>UrlList</c> object/instance.
    ///     </para>
    /// </remarks>
    protected WebScraper(string filePath)
    {
        //  Reads a .yaml file from the filepath specified. Contents are then deserialized.
        var rawYaml = File.ReadAllText(filePath);
        var deserializer = new Deserializer();
        Urls = deserializer.Deserialize<UrlList>(rawYaml).Urls;
    }
    
    /// <summary>
    ///     Web-scrapes each url in the list, and returns a list of <b>RAW</b> string text data from every course
    ///     identified. 
    /// </summary>
    /// <returns>
    ///     A list of string data. Each string is raw unprocessed data that can be converted to a <c>Course</c> object.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method is meant to be used in conjunction with <see cref="TransformToCourses"/> to obtain a list
    ///         of <c>Course</c> objects.
    ///     </para>
    /// </remarks>
    public abstract List<string> ScrapeAll();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    protected abstract List<string> ScrapeWebsite(string url);

    /// <summary>
    ///     Transforms a list of strings into a list of <c>Course</c> objects. Each string must be a representation of a
    ///     course, and each string must be consistent with one another.
    /// </summary>
    /// <param name="rawStrings">
    ///     A list of raw strings to be converted into <c>Course</c> objects.
    /// </param>
    /// <returns>
    ///     A list of <c>Course</c> objects.
    /// </returns>
    protected abstract List<Course> TransformToCourses(List<string> rawStrings);

    
    
    /// <summary>
    ///     A helper class stores a list of urls to be web-scraped.
    /// </summary>
    protected abstract class UrlList
    {
        public List<string> Urls { get; set; } = new List<string>();
    }
}