using System.Diagnostics.CodeAnalysis;
using UniversityDatabase.Core;
using YamlDotNet.Serialization;

namespace UniversityDatabase.Data.Web;

//  TODO: update this documentation when class is finished
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
    /// <summary> The number of attempts when trying to scrape a particular url. </summary>
    protected const uint MaximumAttempts = 3;
    
    /// <summary> The list of urls to be web-scraped. </summary>
    // ReSharper disable once MemberCanBePrivate.Global
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
    /// <seealso cref="TransformToCourses"/>
    public List<string> ScrapeAll()
    {
        return Scrape(Urls);
    }

    /// <summary>
    ///     Web-scrapes a subset of urls in the list, and returns a list of  <b>RAW</b> string text data from every
    ///     course identified.
    /// </summary>
    /// <param name="indexStart"> The zero-based List&lt;T&gt; index at which the range starts. </param>
    /// <param name="number"> The number of elements in the range. </param>
    /// <returns>
    ///     A list of string data. Each string is raw unprocessed data that can be converted to a <c>Course</c> object.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If index is less than 0, or count is less than 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Index and count do not denote a valid range of elements in the List&lt;T&gt;.
    /// </exception>
    /// <seealso cref="TransformToCourses"/>
    public List<string> ScrapeAll(int indexStart, int number)
    {
        return Scrape(Urls.GetRange(indexStart, number));
    }

    /// <summary>
    ///     Web-scrapes a passed list, and returns a list of <b>RAW</b> string text data from every course identified.
    /// </summary>
    /// <param name="urls"> A list of urls to be web-scraped. </param>
    /// <returns>
    ///     A list of string data. Each string is raw unprocessed data that can be converted to a <c>Course</c> object.
    /// </returns>
    /// <seealso cref="TransformToCourses"/>
    protected abstract List<string> Scrape(List<string> urls);

    /// <summary>
    ///     Scrapes a specific url. Returns a list of <b>RAW</b> string text data from every course identified.
    /// </summary>
    /// <param name="url"> The url of the website to scrape </param>
    /// <returns>
    ///     A list of string data. Each string is raw unprocessed data that can be converted to a <c>Course</c> object.
    /// </returns>
    /// <seealso cref="TransformToCourses"/>
    protected abstract List<string> ScrapeWebsite(string url);
    
    
    public abstract Course TransformToCourse(string rawString);

    /// <summary>
    ///     A helper class stores a list of urls to be web-scraped.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class UrlList
    {
        public List<string> Urls { get; set; } = new List<string>();
    }
}