namespace ApplicationLibrary.Data.Repositories;

/// <summary> An interface for abstracting the data layer. </summary>
/// <typeparam name="T"> Type of the class returned from the data layer. </typeparam>
public interface IRepository<T>
{
    T? this[int index] { get; }
    
    /// <summary> Get an <c>IEnumerable&lt;T&gt;</c> object containing <b>all</b> data from the data source. </summary>
    IEnumerable<T> GetAll();

    /// <summary> Whether or not the data source is valid and can be read from safely. </summary>
    bool IsValid();
}