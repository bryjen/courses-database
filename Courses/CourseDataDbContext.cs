using Microsoft.EntityFrameworkCore;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Courses;

public sealed class CourseDataDbContext : 
    DbContext
{
    public List<Course> Courses { get; private set; }

    private DbSet<Course> CoursesData { get; }
    private DbSet<PrerequisiteCourseData> PrerequisitesData { get; }

    public CourseDataDbContext() : base()
    {
        CoursesData = this.Set<Course>();
        PrerequisitesData = this.Set<PrerequisiteCourseData>();

        var prerequisitesDataAsList = PrerequisitesData.ToList();
        Courses = CoursesData.ToList();
        Courses.ForEach(course => course.InitializePrerequisites(prerequisitesDataAsList));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //  Obtaining the connection string from the 'config.yaml' file
        var yaml = File.ReadAllText("config.yaml");
        var deserializer = new DeserializerBuilder().Build();
        var config = deserializer.Deserialize<AppConfig>(yaml);

        optionsBuilder.UseSqlServer(config.DbConnectionString);
    }
}