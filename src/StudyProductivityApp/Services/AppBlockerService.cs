using System.Diagnostics;
using System.Windows.Threading;
using StudyProductivityApp.Interfaces;

namespace StudyProductivityApp.Services
{
    public class AppBlockerService : IAppBlocker
    {
        private readonly DispatcherTimer blockerTimer = new();
        private readonly List<string> blockedProcessesNames = new();

       public bool IsBlocking => blockerTimer.IsEnabled;

       public AppBlockerService()
        {
            blockerTimer.Interval = TimeSpan.FromSeconds(1);
            blockerTimer.Tick += BlockerTimer_Tick;
        }

        public void StartBlocking(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
                return;

            string name = processName.Trim();

            if(!blockedProcessesNames.Contains(name))
                blockedProcessesNames.Add(name);

            BlockProcess();
            blockerTimer.Start();
        }

        public void StopBlocking()
        {
            blockerTimer.Stop();
            blockedProcessesNames.Clear();
        }

        private void BlockerTimer_Tick(object? sender, EventArgs e)
        {
            BlockProcess();
        }

        private void BlockProcess()
        {
            foreach (string blockedProcessName in blockedProcessesNames)
            {
                Process[] processes = Process.GetProcesses()
                .Where(process =>
                {
                    try
                    {
                        return process.ProcessName.Contains(
                            blockedProcessName,
                            StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrWhiteSpace(process.MainWindowTitle) &&
                        process.MainWindowTitle.Contains(
                            blockedProcessName,
                            StringComparison.OrdinalIgnoreCase));
                    }
                    catch
                    {
                        return false;
                    }
                })
                .ToArray();

            foreach (Process process in processes)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        //certain specific window processes cannot be closed
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
        }
    }
}