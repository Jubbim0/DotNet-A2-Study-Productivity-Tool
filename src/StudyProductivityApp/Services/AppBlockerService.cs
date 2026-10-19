using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using StudyProductivityApp.Interfaces;

namespace StudyProductivityApp.Services
{
    public class AppBlockerService : IAppBlocker
    {
        private readonly DispatcherTimer blockerTimer = new() { Interval = TimeSpan.FromSeconds(1) };
        private readonly HashSet<string> blockedNames = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<(int Id, long StartTicks)> existingProcesses = new();
        private readonly HashSet<int> existingProcessIds = new();

        private static readonly HashSet<string> ExcludedNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "explorer", "applicationframehost", "shellexperiencehost", "startmenuexperiencehost",
            "searchhost", "searchapp", "textinputhost", "sihost", "dwm", "csrss",
            "winlogon", "wininit", "services", "lsass", "smss", "svchost",
            "runtimebroker", "taskhostw", "taskmgr", "system", "registry",
            "fontdrvhost", "audiodg", "ctfmon", "conhost", "dllhost",
            "securityhealthservice", "securityhealthsystray", "msmpeng", "swusb",
            "calculatorapp" // Packaged Calculator often retains background/host windows.
        };

        // Notepad is a user application even when its executable lives under Windows.
        private static readonly HashSet<string> AllowedWindowsApps = new(StringComparer.OrdinalIgnoreCase)
        {
            "notepad"
        };

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);
        private delegate bool EnumWindowsProc(IntPtr window, IntPtr parameter);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr window);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr window);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr window, uint command);
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr window, int attribute, out int value, int size);

        private const uint GW_OWNER = 4;
        private const int DWMWA_CLOAKED = 14;

        public bool IsBlocking => blockerTimer.IsEnabled;

        public AppBlockerService()
        {
            blockerTimer.Tick += (_, _) => BlockNewProcesses();
        }

        public static bool IsAllowedTarget(Process process)
        {
            try
            {
                if (process.Id == Environment.ProcessId || process.HasExited ||
                    ExcludedNames.Contains(process.ProcessName)) return false;

                string? path = process.MainModule?.FileName;
                if (string.IsNullOrWhiteSpace(path)) return false;

                string fullPath = Path.GetFullPath(path);
                string windowsDirectory = Path.GetFullPath(
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows))
                    .TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

                if (fullPath.StartsWith(windowsDirectory, StringComparison.OrdinalIgnoreCase) &&
                    !AllowedWindowsApps.Contains(process.ProcessName)) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static HashSet<int> GetVisibleAppProcessIds()
        {
            var ids = new HashSet<int>();
            EnumWindows((window, _) =>
            {
                if (!IsWindowVisible(window) || GetWindowTextLength(window) == 0 ||
                    GetWindow(window, GW_OWNER) != IntPtr.Zero) return true;

                if (DwmGetWindowAttribute(window, DWMWA_CLOAKED, out int cloaked,
                    sizeof(int)) == 0 && cloaked != 0) return true;

                GetWindowThreadProcessId(window, out uint pid);
                if (pid != 0 && pid <= int.MaxValue) ids.Add((int)pid);
                return true;
            }, IntPtr.Zero);
            return ids;
        }

        public static List<string> GetSelectableApps()
        {
            var visibleIds = GetVisibleAppProcessIds();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Process process in Process.GetProcesses())
            {
                using (process)
                {
                    try
                    {
                        if (visibleIds.Contains(process.Id) && IsAllowedTarget(process))
                            names.Add(process.ProcessName);
                    }
                    catch { /* Ignore inaccessible/exited processes. */ }
                }
            }
            return names.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public void StartBlocking(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName)) return;
            string name = processName.Trim();
            if (!GetSelectableApps().Contains(name, StringComparer.OrdinalIgnoreCase)) return;

            blockedNames.Add(name);
            if (blockerTimer.IsEnabled) return;

            existingProcesses.Clear();
            existingProcessIds.Clear();
            foreach (Process process in Process.GetProcesses())
            {
                using (process)
                {
                    existingProcessIds.Add(process.Id);
                    try { existingProcesses.Add((process.Id, process.StartTime.Ticks)); }
                    catch { /* Inaccessible processes are not trusted as new. */ }
                }
            }
            blockerTimer.Start();
        }

        public void StopBlocking()
        {
            blockerTimer.Stop();
            blockedNames.Clear();
            existingProcesses.Clear();
            existingProcessIds.Clear();
        }

        private void BlockNewProcesses()
        {
            if (blockedNames.Count == 0) return;
            foreach (Process process in Process.GetProcesses())
            {
                using (process)
                {
                    try
                    {
                        if (!blockedNames.Contains(process.ProcessName) || !IsAllowedTarget(process)) continue;
                        var identity = (process.Id, process.StartTime.Ticks);
                        if (existingProcesses.Contains(identity) || existingProcessIds.Contains(process.Id)) continue;
                        process.Kill();
                    }
                    catch (Exception ex) when (ex is InvalidOperationException ||
                                               ex is System.ComponentModel.Win32Exception ||
                                               ex is UnauthorizedAccessException)
                    {
                        // Do not elevate privileges or interfere with protected processes.
                    }
                }
            }
        }
    }
}
