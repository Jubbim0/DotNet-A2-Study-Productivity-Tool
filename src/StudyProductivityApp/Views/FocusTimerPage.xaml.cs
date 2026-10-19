using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;

namespace StudyProductivityApp.Views
{
    public partial class FocusPage : Page
    {
        private readonly StudySessionService sessionService = new();
        private readonly DispatcherTimer timer = new();
        private readonly DispatcherTimer appListRefreshTimer = new() { Interval = TimeSpan.FromSeconds(2) };
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
            Loaded += FocusPage_Loaded;
            appListRefreshTimer.Tick += (_, _) => LoadRunningApps();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            LoadCategories();
            LoadHistory();
            LoadRunningApps();
            BlockedAppInput.DropDownOpened += (_, _) => LoadRunningApps();
            UpdateDisplay();


            PauseButton.IsEnabled = false;
        }

        private void FocusPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRunningApps();
            appListRefreshTimer.Start();
        }

        private void FocusPage_Unloaded(object sender, RoutedEventArgs e)
        {
            appListRefreshTimer.Stop();
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
            string? selected = BlockedAppInput.SelectedItem as string;
            var available = AppBlockerService.GetSelectableApps();
            if (BlockedAppInput.ItemsSource is IEnumerable<string> old && old.SequenceEqual(available))
                return;

            BlockedAppInput.ItemsSource = available;
            if (selected != null && available.Contains(selected, StringComparer.OrdinalIgnoreCase))
                BlockedAppInput.SelectedItem = available.First(x => string.Equals(x, selected, StringComparison.OrdinalIgnoreCase));
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

            if (!AppBlockerService.GetSelectableApps().Contains(selectedApp, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("That application is not an allowed blocking target.");
                LoadRunningApps();
                return;
            }

            if (blockedApps.Contains(selectedApp, StringComparer.OrdinalIgnoreCase))
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

            // Start monitoring only after all targets have been selected.
            // Existing processes are preserved by AppBlockerService.
            foreach (string app in blockedApps)
                appBlocker.StartBlocking(app);


            timer.Start();
            isPaused = false;

            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            DurationInput.IsEnabled = false;
            CategoryInput.IsEnabled = false;
            BlockedAppInput.IsEnabled = false;
            RemoveBlockedAppButton.IsEnabled = false;
            AddBlockedAppButton.IsEnabled = false;
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
            AddBlockedAppButton.IsEnabled = true;
            RemoveBlockedAppButton.IsEnabled = true;

            UpdateDisplay();
        }

        // Update countdown display
        private void UpdateDisplay()
        {
            TimerDisplay.Text = $"{(int)remainingTime.TotalMinutes:D2}:{remainingTime.Seconds:D2}";
        }
    }
}