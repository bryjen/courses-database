using System.Text.Json;
using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Repositories;

/// <summary>
///     This class reads a <c>.json</c> file then attempts to de-serialize contents into a <c>IEnumerable&lt;Course&gt;</c>
///     object.
/// </summary>
public class CourseRepositoryDeserializer : DbContext, IRepository<Course>
{
    private readonly List<Course> _courses;

    /// <summary> Instantiates a new <c>CourseRepositoryDeserializer</c> object by de-serializing a .json file. </summary>
    public CourseRepositoryDeserializer(string filepath)
    {
        var rawJson = File.ReadAllText(filepath);
        var deserializedData = JsonSerializer.Deserialize<List<Course>>(rawJson);

        if (deserializedData is null)
            throw new FileLoadException(
                $"An error occurred while trying to de-serialize contents of the file: {filepath}\n" +
                "Maybe the file is outdated/incompatible with the current definition of \"Course\"");

        _courses = deserializedData.ToList();
    }
    
    public Course? this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IEnumerable<Course> GetAll()
    {
        return _courses;
    }

    public bool IsValid()
    {
        return true;    //  If the object is successfully initialized, it means de-serialization was successful and
                        //  contents are valid.
    }
}