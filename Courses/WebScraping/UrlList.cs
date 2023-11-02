namespace Courses.WebScraping;


/// <summary> Class containing a list of urls to be web-scraped. Usually stored and loaded from an external file in
/// .json/.xml/.yaml.</summary>
internal class UrlList
{
    public List<string> Urls { get; set; } = new List<string>();
}