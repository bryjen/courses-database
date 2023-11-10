namespace ApplicationLibrary.Data.Repositories;

public interface IRepository<T>
{
    T? this[int index] { get; set; }
    
    IEnumerable<T> GetAll();

    /// <summary> Whether or not the data source is valid and can be read from safely. </summary>
    bool IsValid();
}