using ApplicationLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLibrary.Data.Database;

/// <summary> Class to override, set, and load course prerequisite data to the db server. </summary>
/// <remarks> Identity column is to be reset, higher level permissions must be granted to the db user/viewer or else
///           some error/exception will be thrown.</remarks>
public class PrereqCourseTableManager : TableManager<Course.PrerequisiteCourseData>
{
    internal DbSet<Course.PrerequisiteCourseData>? PrerequisiteCourses { get; set; }

    public PrereqCourseTableManager(string dbConnectionString) : base(dbConnectionString) { }

    public override void ReplaceAllWith(IEnumerable<Course.PrerequisiteCourseData> newEntries)
    {
        if (PrerequisiteCourses is null)
            return;

        var existingEntities = PrerequisiteCourses.ToList();
        PrerequisiteCourses.RemoveRange(existingEntities);

        //  Requires higher level permissions for the database.
        this.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('courses-prerequisites', RESEED, 0);");
        
        PrerequisiteCourses.AddRange(newEntries);
        SaveChanges();
    }

    public override IEnumerable<Course.PrerequisiteCourseData> GetAllEntities()
    {
        return PrerequisiteCourses!.ToList();
    }
}