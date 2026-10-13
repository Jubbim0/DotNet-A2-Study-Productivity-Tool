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

            StudyCalendar.SelectedDate = DateTime.Today;

            LoadTasksForDate(DateTime.Today);
        }

        private void StudyCalendar_SelectedDatesChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (TasksDataGrid == null || SelectedDateText == null)
                return;

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

            var tasks = db.Tasks
                .AsNoTracking()
                .Where(t => t.DueDate >= date.Date &&
                            t.DueDate < nextDate)
                .OrderBy(t => t.Title)
                .ToList();

            TasksDataGrid.ItemsSource = tasks;
        }
    }
}