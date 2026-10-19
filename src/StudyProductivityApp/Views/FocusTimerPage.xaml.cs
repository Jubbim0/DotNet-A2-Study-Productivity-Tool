using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;
using System.Diagnostics;

namespace StudyProductivityApp.Views
{
    public partial class FocusPage : Page
    {
        private readonly StudySessionService sessionService = new();
        private readonly DispatcherTimer timer = new();
        private readonly AppBlockerService appBlocker = new();

        private readonly List<string> blockedApps = new();

        private TimeSpan remainingTime;
        private int sessionMinutes;
        private int? sessionCategoryId;
        private bool isPaused;

        public FocusPage()
        {
            InitializeComponent();

            Unloaded += FocusPage_Unloaded;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            LoadCategories();
            LoadHistory();
            LoadRunningApps();
            UpdateDisplay();


            PauseButton.IsEnabled = false;
        }

        private void FocusPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ResetTimer();
        }

        // Load categories
        private void LoadCategories()
        {
            using AppDbContext db = new();

            CategoryInput.ItemsSource = db.Categories
                .OrderBy(category => category.Name)
                .ToList();
        }

        private void LoadRunningApps()
        {
            var runningApps = Process.GetProcesses()
                .Where(process =>
                {
                    try
                    {
                        return !string.IsNullOrWhiteSpace(process.MainWindowTitle);
                    }
                    catch
                    {
                        return false;
                    }
                })
                .Select(process => process.ProcessName)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            BlockedAppInput.ItemsSource = runningApps;
        }

        // Load completed sessions
        private void LoadHistory()
        {
            HistoryGrid.ItemsSource = sessionService.GetSessions();
        }

        public void AddBlockedApp_Click(object sender, RoutedEventArgs e)
        {
            if (BlockedAppInput.SelectedItem is not string selectedApp)
                return;

            if (blockedApps.Contains(selectedApp))
                return;

            blockedApps.Add(selectedApp);

            BlockedAppsList.ItemsSource = null;
            BlockedAppsList.ItemsSource = blockedApps;
        }

        private void RemoveBlockedApp_Click(object sender, RoutedEventArgs e)
        {
            if (BlockedAppsList.SelectedItem is not string selectedApp)
                return;

            blockedApps.Remove(selectedApp);

            BlockedAppsList.ItemsSource = null;
            BlockedAppsList.ItemsSource = blockedApps;
        }

        // Start or resume timer
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
                return;

            if (!isPaused)
            {
                if (!int.TryParse(DurationInput.Text, out int minutes) || minutes <= 0)
                {
                    MessageBox.Show("Please enter a valid duration.");
                    return;
                }

                sessionMinutes = minutes;
                sessionCategoryId = CategoryInput.SelectedValue as int?;
                remainingTime = TimeSpan.FromMinutes(minutes);

                UpdateDisplay();
            }

            foreach (string app in blockedApps)
            {
                appBlocker.StartBlocking(app);
            }


            timer.Start();
            isPaused = false;

            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            DurationInput.IsEnabled = false;
            CategoryInput.IsEnabled = false;
            BlockedAppInput.IsEnabled = false;
            RemoveBlockedAppButton.IsEnabled = false;
        }

   
        // Pause timer
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            appBlocker.StopBlocking();
            isPaused = true;

            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            BlockedAppInput.IsEnabled = true;
            AddBlockedAppButton.IsEnabled = true;
            RemoveBlockedAppButton.IsEnabled = true;
            AddBlockedAppButton.IsEnabled = true;
            RemoveBlockedAppButton.IsEnabled = true;
        }

        // Reset timer
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer();
        }

        // Timer countdown
        private void Timer_Tick(object? sender, EventArgs e)
        {
            remainingTime -= TimeSpan.FromSeconds(1);

            if (remainingTime <= TimeSpan.Zero)
            {
                StudySession session = new()
                {
                    CompletedAt = DateTime.Now,
                    DurationMinutes = sessionMinutes,
                    CategoryId = sessionCategoryId
                };

                ResetTimer();
                sessionService.AddSession(session);
                LoadHistory();

                MessageBox.Show("Focus session completed!");
                return;
            }

            UpdateDisplay();
        }

        // Reset timer and controls
        private void ResetTimer()
        {
            timer.Stop();
            appBlocker.StopBlocking();

            isPaused = false;
            remainingTime = TimeSpan.Zero;

            DurationInput.IsEnabled = true;
            CategoryInput.IsEnabled = true;
            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            BlockedAppInput.IsEnabled = true;

            UpdateDisplay();
        }

        // Update countdown display
        private void UpdateDisplay()
        {
            TimerDisplay.Text = $"{(int)remainingTime.TotalMinutes:D2}:{remainingTime.Seconds:D2}";
        }
    }
}