The entire web-application is made up of several smaller components:
1. The Application Library
2. The Web API **(To be developed)**
3. The Frontend **(To be developed)**
4. Separate Database Manager 

---

<div align="center">
    <p align="center">
        <h2 align="center">Application Library</h2>
        This library provides the functionalities for all of the other application components.
        <br>
        Below are a list of the common functionalities the library provides.
</div>

### Database Interaction
Data is hosted using an Azure SQL database, and read-only access is abstracted using repository implementations.

```csharp
IRepository<Course> courseRepo = new CourseRepositoryDatabase(AppSettings.DbConnectionString);

//  Get an IEnumerable<Course> of all courses in the database
IEnumerable<Course> courses = courseRepo.GetAll();
```

For more complex functionality, with complex insert/update/delete operations, such classes are slightly more coupled with their related services.

```csharp
namespace ApplicationLibrary.Services;

/// 'API' for interacting with the users database
public class UserServices
{
    public void CreateUser(...) { ... }
    
    ...
}

/// Class that performs low-level CRUD operation logic on the database
internal class UsersManager : DbContext
{
    ...
}
```
---

<div align="center">
    <p align="center">
        <h2 align="center">Database Manager</h2>
        Abstracts low level data manipulation of tables in the database. 
        <br>
</div>