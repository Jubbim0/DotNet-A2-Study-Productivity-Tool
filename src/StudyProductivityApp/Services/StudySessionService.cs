using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;

namespace StudyProductivityApp.Services
{
    public class StudySessionService
    {
        // Save a completed study session
        public void AddSession(StudySession session)
        {
            using AppDbContext db = new();

            db.StudySessions.Add(session);
            db.SaveChanges();
        }

        // Retrieve completed study sessions
        public List<StudySession> GetSessions()
        {
            using AppDbContext db = new();

            return db.StudySessions
                .Include(session => session.Category)
                .AsNoTracking()
                .OrderByDescending(session => session.CompletedAt)
                .ToList();
        }
    }
}