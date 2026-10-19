using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;

namespace StudyProductivityApp.Views
{
    public partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();

            // Default to today's date when the calendar opens.
            StudyCalendar.SelectedDate = DateTime.Today;

            LoadTasksForDate(DateTime.Today);
        }

        private void StudyCalendar_SelectedDatesChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (TasksDataGrid == null || SelectedDateText == null)
                return;

            // Refresh the task list when a different date is selected.
            if (StudyCalendar.SelectedDate is DateTime selectedDate)
            {
                LoadTasksForDate(selectedDate);
            }
        }

        private void LoadTasksForDate(DateTime date)
        {
            SelectedDateText.Text = date.ToString("dddd, dd MMMM yyyy");

            using AppDbContext db = new();

            DateTime nextDate = date.Date.AddDays(1);

            // Retrieve tasks due on the selected date, regardless of time.
            var tasks = db.Tasks
                .AsNoTracking()
                .Where(t => t.DueDate >= date.Date &&
                            t.DueDate < nextDate)
                .OrderBy(t => t.Title)
                .ToList();

            // Show an empty-state message when no tasks are found.
            TasksDataGrid.ItemsSource = tasks;
            CalendarTasksEmptyText.Visibility = tasks.Count == 0
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
        }
    }
}