using StudyProductivityApp.Data;
using StudyProductivityApp.Models;

namespace StudyProductivityApp.Services
{
    public class StatisticsService
    {
        private readonly Func<AppDbContext> createContext;

        public StatisticsService() : this(() => new AppDbContext())
        {
        }

        public StatisticsService(Func<AppDbContext> createContext)
        {
            this.createContext = createContext ?? throw new ArgumentNullException(nameof(createContext));
        }

        // Total minutes spent studying
        public int GetTotalStudyMinutes()
        {
            using AppDbContext db = createContext();

            return db.StudySessions.Sum(session => session.DurationMinutes);
        }

        // Number of completed study sessions
        public int GetCompletedSessions()
        {
            using AppDbContext db = createContext();

            return db.StudySessions.Count();
        }

        // Average study session duration
        public double GetAverageSessionMinutes()
        {
            using AppDbContext db = createContext();

            return db.StudySessions
                .Select(session => (double?)session.DurationMinutes)
                .Average() ?? 0;
        }

        // Percentage of completed tasks
        public double GetTaskCompletionRate()
        {
            using AppDbContext db = createContext();

            int totalTasks = db.Tasks.Count();

            if (totalTasks == 0)
                return 0;

            int completedTasks = db.Tasks.Count(task => task.Status == Models.TaskStatus.Completed);

            return (double)completedTasks / totalTasks * 100;
        }

        // Total study minutes grouped by category
        public Dictionary<string, int> GetStudyTimeByCategory()
        {
            using AppDbContext db = createContext();

            return db.StudySessions
                .Select(session => new
                {
                    CategoryName = session.Category != null
                        ? session.Category.Name
                        : "Uncategorised",
                    session.DurationMinutes
                })
                .ToList()
                .GroupBy(session => session.CategoryName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(session => session.DurationMinutes));
        }

        // Total study minutes grouped by date
        public Dictionary<DateTime, int> GetDailyStudyTime()
        {
            using AppDbContext db = createContext();

            return db.StudySessions
                .ToList()
                .GroupBy(session => session.CompletedAt.Date)
                .OrderBy(group => group.Key)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(session => session.DurationMinutes));
        }
    }
}