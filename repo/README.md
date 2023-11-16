Local repository of serialized `IEnumerable` types of entity classes. 
Stores 'snapshots' of data from the database that can be deserialized to avoid the overhead of accessing the data multiple times in a short time while (unit) testing.

### De-serializing data:
```csharp
string filePath = 
```