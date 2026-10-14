using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;

namespace StudyProductivityApp.Tests;

[TestFixture]
public class StudySessionTests : SqliteTestBase
{
    private StudySessionService Service => new(CreateContext);

    [Test]
    public void AddSession_ValidSession_AddsSessionToDatabase()
    {
        // Arrange
        var session = new StudySession
        {
            DurationMinutes = 60,
            CompletedAt = DateTime.Now
        };
        // Act
        Service.AddSession(session);
        // Assert
        using var context = CreateContext();
        var addedSession = context.StudySessions.FirstOrDefault(s => s.DurationMinutes == 60);
        Assert.IsNotNull(addedSession);
    }

    [Test]
    public void GetSessions_WithSessions_ReturnsSessionsInDescendingOrder()
    {
        // Arrange
        using var context = CreateContext();
        context.StudySessions.Add(new StudySession { DurationMinutes = 30, CompletedAt = DateTime.Now.AddHours(-1) });
        context.StudySessions.Add(new StudySession { DurationMinutes = 45, CompletedAt = DateTime.Now });
        context.SaveChanges();
        // Act
        var sessions = Service.GetSessions();
        // Assert
        NUnit.Framework.Assert.That(sessions.Count, Is.EqualTo(2));
        Assert.IsTrue(sessions[0].CompletedAt > sessions[1].CompletedAt);
    }
}