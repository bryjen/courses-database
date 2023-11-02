using Courses;
using Microsoft.Data.SqlClient;

public static class Program
{
    public static void Main(string[] args)
    {
        using (var context = new CourseDataDbContext())
        {
            var allRows = context.Courses.ToList();
            //var prereqRows = context.PrerequisiteCourseData.ToList();

            /*
            var filteredRows =
                from course in allRows
                where course.Number == 498 && course.Type == "ENCS"
                select course;
            foreach (var course in filteredRows)
            {
                Console.WriteLine($"{course.Type} {course.Number} - {course.Name} -> {course.Components}");
                Console.WriteLine(course.Components == null);
            }
             */

            Console.WriteLine("kldfjdskjfslkjflksdjgflksg");
            //allRows.ForEach(course => Console.WriteLine($"{course.Type} {course.Number} - {course.Name} -> {course.Components}"));
            //prereqRows.ForEach(Console.WriteLine);
        }
    }
}
