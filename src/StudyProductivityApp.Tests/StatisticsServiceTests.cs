using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;
using TaskStatus = StudyProductivityApp.Models.TaskStatus;

namespace StudyProductivityApp.Tests;

[TestFixture]
public class StatisticsServiceTests : SqliteTestBase
{
    private StatisticsService Service => new(CreateContext);

    [Test]
    public void GetTotalStudyMinutes_NoSessions_ReturnsZero()
    {
        // Act
        int totalMinutes = Service.GetTotalStudyMinutes();
        // Assert
        NUnit.Framework.Assert.That(totalMinutes, Is.EqualTo(0));
    }

    [Test]
    public void GetTotalStudyMinutes_WithSessions_ReturnsCorrectTotal()
    {
        // Arrange
        using var context = CreateContext();
        context.StudySessions.Add(new StudySession { DurationMinutes = 30 });
        context.StudySessions.Add(new StudySession { DurationMinutes = 45 });
        context.SaveChanges();
        // Act
        int totalMinutes = Service.GetTotalStudyMinutes();
        // Assert
        NUnit.Framework.Assert.That(totalMinutes, Is.EqualTo(75));
    }

    [Test]
    public void GetCompletedSessions_NoSessions_ReturnsZero()
    {
        // Act
        int completedSessions = Service.GetCompletedSessions();
        // Assert
        NUnit.Framework.Assert.That(completedSessions, Is.EqualTo(0));
    }

    [Test]
    public void GetCompletedSessions_WithSessions_ReturnsCorrectCount()
    {
        // Arrange
        using var context = CreateContext();
        context.StudySessions.Add(new StudySession { DurationMinutes = 30 });
        context.StudySessions.Add(new StudySession { DurationMinutes = 45 });
        context.SaveChanges();
        // Act
        int completedSessions = Service.GetCompletedSessions();
        // Assert
        NUnit.Framework.Assert.That(completedSessions, Is.EqualTo(2));
    }

    [Test]
    public void GetAverageSessionMinutes_NoSessions_ReturnsZero()
    {
        // Act
        double averageMinutes = Service.GetAverageSessionMinutes();
        // Assert
        NUnit.Framework.Assert.That(averageMinutes, Is.EqualTo(0));
    }

    [Test]
    public void GetAverageSessionMinutes_WithSessions_ReturnsCorrectAverage()
    {
        // Arrange
        using var context = CreateContext();
        context.StudySessions.Add(new StudySession { DurationMinutes = 30 });
        context.StudySessions.Add(new StudySession { DurationMinutes = 60 });
        context.SaveChanges();
        // Act
        double averageMinutes = Service.GetAverageSessionMinutes();
        // Assert
        NUnit.Framework.Assert.That(averageMinutes, Is.EqualTo(45));
    }

    [Test]
    public void GetTaskCompletionRate_NoTasks_ReturnsZero()
    {
        // Act
        double completionRate = Service.GetTaskCompletionRate();
        // Assert
        NUnit.Framework.Assert.That(completionRate, Is.EqualTo(0));
    }

    [Test]
    public void GetTaskCompletionRate_WithTasks_ReturnsCorrectRate()
    {
        // Arrange
        using var context = CreateContext();
        context.Tasks.Add(new StudyTask { Status = TaskStatus.Completed });
        context.Tasks.Add(new StudyTask { Status = TaskStatus.NotStarted });
        context.SaveChanges();
        // Act
        double completionRate = Service.GetTaskCompletionRate();
        // Assert
        NUnit.Framework.Assert.That(completionRate, Is.EqualTo(50));
    }
}