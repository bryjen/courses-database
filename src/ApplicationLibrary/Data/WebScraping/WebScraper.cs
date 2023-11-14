using System.Text.Json;
using ApplicationLibrary.Data.Entities;

namespace ApplicationLibrary.Data.WebScraping;

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
public abstract class WebScraper
{
    /// <summary> The number of attempts when trying to scrape a particular url. </summary>
    protected const uint MaximumAttempts = 3;
    
    /// <summary> The list of urls to be web-scraped. </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public List<string> Urls { get; private set; }

    /// <summary> Constructs a WebScraper object. Takes in the location for a .json file containing the list of urls to scrape.</summary>
    protected WebScraper(string filePath)
    {
        var rawJson = File.ReadAllText(filePath);
        Urls = JsonSerializer.Deserialize<UrlList>(rawJson)!.Urls;
    }

    /// <summary> Web-scrapes each url in the list, and returns a list of <b>RAW</b> HTML string data from every course identified. </summary>
    public List<string> ScrapeAll()
    {
        return Scrape(Urls);
    }

    /// <summary> Web-scrapes <b>a subset</b> of urls in the list, and returns a list of <b>RAW</b> HTML string data from every course identified. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> If index is less than 0, or count is less than 0. </exception>
    /// <exception cref="ArgumentException"> Index and count do not denote a valid range of elements in the List&lt;T&gt;. </exception>
    public List<string> ScrapeAll(int indexStart, int number)
    {
        return Scrape(Urls.GetRange(indexStart, number));
    }

    /// <summary> Web-scrapes a list of urls, and returns a list of <b>RAW</b> HTML string data from every course identified. </summary>
    protected abstract List<string> Scrape(List<string> urls);

    /// <summary> Scrapes a specific url. Returns a list of <b>RAW</b> string text data from every course identified. </summary>
    public abstract List<string> ScrapeWebsite(string url);
    
    /// <summary> Parses raw HTML data into a <code>Course</code> object. </summary>
    public abstract Course TransformToCourse(string rawString);

    /// <summary> A helper class stores a list of urls to be web-scraped. </summary>
    internal class UrlList
    {
        public List<string> Urls { get; set; } = new List<string>();
    }
}