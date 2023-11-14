using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Database;

public class CourseTableManager : TableManager<Course>
{
    internal DbSet<Course>? Courses { get; set; }

    public CourseTableManager(string dbConnectionString) : base(dbConnectionString) { }

    public override void ReplaceAllWith(IEnumerable<Course> newEntries)
    {
        if (Courses is null)
            return;
        
        var existingEntities = Courses.ToList();
        Courses.RemoveRange(existingEntities);
        Courses.AddRange(newEntries);
        SaveChanges();
    }

    public override IEnumerable<Course> GetAllEntities()
    {
        return Courses!.ToList();
    }
}