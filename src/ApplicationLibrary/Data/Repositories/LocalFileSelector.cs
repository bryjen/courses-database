using System.Globalization;
using ApplicationLibrary.Config;

namespace ApplicationLibrary.Data.Repositories;

/// <summary>
///     A 'helper' class that selects the most recent serialized <c>.json</c> file in the project's local repository of
///     stored serialized object files.
/// </summary>
public static class LocalFileSelector
{
    private static readonly string FilesDirectory = $"{AppSettings.SolutionDirectory}/repo/";

    /// <summary>
    ///     Returns the file name of the most recently serialized <code>.json</code> file containing <b>course</b>
    ///     information in the local serialized objects repository.
    /// </summary>
    public static string? GetSerializedCoursesFileName()
    {
        return GetMostRecentFile("courses");
    }

    /// <summary>
    ///     Takes a prefix and returns the most recent file obeying the format <c>fileNamePrefix_YYYY_MM_DD</c> 
    /// </summary>
    private static string? GetMostRecentFile(string fileNamePrefix)
    {
        var files = Directory.GetFiles(FilesDirectory, $"{fileNamePrefix}_*.json");
        var mostRecentDate = DateTime.MinValue;
        string? mostRecentFile = null;

        foreach (var fileName in files)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (TryParseDateFromFileName(fileNameWithoutExtension, out DateTime fileDate) 
                && fileDate > mostRecentDate)
            {
                mostRecentDate = fileDate;
                mostRecentFile = fileName;
            }
        }

        return mostRecentFile;
    }
    
    //  Helper function that parses the formatted date formatted into the file name and outputs the formatted date.
    private static bool TryParseDateFromFileName(string fileName, out DateTime date)
    {
        date = DateTime.MinValue;
        var tokens = fileName.Split("_");
        
        if (tokens.Length != 4)
            return false;
        
        var dateString = $"{tokens[1]}-{tokens[2].PadLeft(2, '0')}-{tokens[3].PadLeft(2, '0')}";; 
        return DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }
}