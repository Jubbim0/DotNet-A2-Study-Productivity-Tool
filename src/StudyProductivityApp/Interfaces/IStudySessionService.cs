using StudyProductivityApp.Models;

namespace StudyProductivityApp.Interfaces
{
    public interface IStudySessionService
    {
        void AddSession(StudySession session);

        List<StudySession> GetSessions();
    }
}