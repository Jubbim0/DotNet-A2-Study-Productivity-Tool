namespace StudyProductivityApp.Interfaces
{
    public interface IStatisticsService
    {
        int GetTotalStudyMinutes();

        int GetCompletedSessions();

        double GetAverageSessionMinutes();

        double GetTaskCompletionRate();

        Dictionary<string, int> GetStudyTimeByCategory();

        Dictionary<DateTime, int> GetDailyStudyTime();
    }
}