using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Interfaces;

namespace StudyProductivityApp.Services
{
    public class StudySessionService : IStudySessionService
    {
        private readonly Func<AppDbContext> createContext;

        public StudySessionService() : this(() => new AppDbContext())
        {
        }

        public StudySessionService(Func<AppDbContext> createContext)
        {
            this.createContext = createContext ?? throw new ArgumentNullException(nameof(createContext));
        }

        // Save a completed study session
        public void AddSession(StudySession session)
        {
            using AppDbContext db = createContext();

            db.StudySessions.Add(session);
            db.SaveChanges();
        }

        // Retrieve completed study sessions
        public List<StudySession> GetSessions()
        {
            using AppDbContext db = createContext();

            return db.StudySessions
                .Include(session => session.Category)
                .AsNoTracking()
                .OrderByDescending(session => session.CompletedAt)
                .ToList();
        }
    }
}