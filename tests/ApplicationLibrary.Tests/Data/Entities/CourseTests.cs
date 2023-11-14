using ApplicationLibrary.Data.Entities;

namespace ApplicationLibrary.Tests.Data.Entities;

[TestFixture]
public class CourseTests
{
    [Test]
    [Description("Tests whether a prereq object can split successfully. Test 1.")]
    public void TestPrerequisiteSplitting1()
    {
        var prereq = new Course.PrerequisiteCourseData(1, "COMP", 228, "COMP 248~MATH 203~MATH 204");
        var prereqSplit = prereq.SplitPrerequisiteCourseData().ToList();
        
        Console.WriteLine($"Input:\n{prereq}"); 
        Console.WriteLine("\nResults:");
        prereqSplit.ForEach(Console.WriteLine);
        
        Assert.Multiple(() =>
        {
            Assert.That(prereqSplit, Has.Count.EqualTo(3));
            
            Assert.That(prereqSplit[0].UniversityId, Is.EqualTo(1));
            Assert.That(prereqSplit[0].Type, Is.EqualTo("COMP"));
            Assert.That(prereqSplit[0].Number, Is.EqualTo(228));
            Assert.That(prereqSplit[0].PrerequisitesString, Is.EqualTo("COMP 248"));
            
            Assert.That(prereqSplit[1].UniversityId, Is.EqualTo(1));
            Assert.That(prereqSplit[1].Type, Is.EqualTo("COMP"));
            Assert.That(prereqSplit[1].Number, Is.EqualTo(228));
            Assert.That(prereqSplit[1].PrerequisitesString, Is.EqualTo("MATH 203"));
            
            Assert.That(prereqSplit[2].UniversityId, Is.EqualTo(1));
            Assert.That(prereqSplit[2].Type, Is.EqualTo("COMP"));
            Assert.That(prereqSplit[2].Number, Is.EqualTo(228));
            Assert.That(prereqSplit[2].PrerequisitesString, Is.EqualTo("MATH 204"));
        });
    }

    [Test]
    [Description("Tests whether a prereq object can split successfully. Test 2.")]
    public void TestPrerequisiteSplitting2()
    {
        var prereq = new Course.PrerequisiteCourseData(1, "COMP", 208, "COMP 108");
        var prereqSplit = prereq.SplitPrerequisiteCourseData().ToList();
        
        Console.WriteLine($"Input:\n{prereq}"); 
        Console.WriteLine("\nResults:");
        prereqSplit.ForEach(Console.WriteLine);
        
        Assert.Multiple(() =>
        {
            Assert.That(prereqSplit, Has.Count.EqualTo(1));
            
            Assert.That(prereqSplit[0].UniversityId, Is.EqualTo(1));
            Assert.That(prereqSplit[0].Type, Is.EqualTo("COMP"));
            Assert.That(prereqSplit[0].Number, Is.EqualTo(208));
            Assert.That(prereqSplit[0].PrerequisitesString, Is.EqualTo("COMP 108"));
        });
    }
    
    
    [Test]
    [Description("Tests whether a prereq object can split successfully. Test 3.")]
    public void TestPrerequisiteSplitting3()
    {
        var prereq = new Course.PrerequisiteCourseData(1, "COMP", 353, "COMP 232/COEN 231~COMP 352/COEN 352");
        var prereqSplit = prereq.SplitPrerequisiteCourseData().ToList();
        
        Console.WriteLine($"Input:\n{prereq}"); 
        Console.WriteLine("\nResults:");
        prereqSplit.ForEach(Console.WriteLine);
        
        Assert.Multiple(() =>
        {
            Assert.That(prereqSplit, Has.Count.EqualTo(2));
            
            Assert.That(prereqSplit[0].UniversityId, Is.EqualTo(1));
            Assert.That(prereqSplit[0].Type, Is.EqualTo("COMP"));
            Assert.That(prereqSplit[0].Number, Is.EqualTo(353));
            Assert.That(prereqSplit[0].PrerequisitesString, Is.EqualTo("COMP 232/COEN 231"));
            
            Assert.That(prereqSplit[1].UniversityId, Is.EqualTo(1));
            Assert.That(prereqSplit[1].Type, Is.EqualTo("COMP"));
            Assert.That(prereqSplit[1].Number, Is.EqualTo(353));
            Assert.That(prereqSplit[1].PrerequisitesString, Is.EqualTo("COMP 352/COEN 352"));
        });
    }
}