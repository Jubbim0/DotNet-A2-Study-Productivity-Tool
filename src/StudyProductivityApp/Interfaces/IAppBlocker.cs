namespace StudyProductivityApp.Interfaces
{
    public interface IAppBlocker
    {
        void StartBlocking(string processName);

        void StopBlocking();

        bool IsBlocking { get; }
    }
}