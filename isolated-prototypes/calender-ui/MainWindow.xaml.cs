using System.Windows;

namespace CalendarPrototype
{
    public partial class MainWindow : Window
    {
        private readonly List<StudyTask> tasks =
        [
            new("C# Assignment 2", new DateTime(2026, 10, 16)),
            new("Cloud Computing Report", new DateTime(2026, 10, 20)),
            new("Image Processing Project", new DateTime(2026, 10, 23)),
            new("C# Quiz", new DateTime(2026, 10, 23))
        ];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StudyCalendar_SelectedDatesChanged(object sender, EventArgs e)
        {
            if (StudyCalendar.SelectedDate == null)
                return;

            DateTime selectedDate = StudyCalendar.SelectedDate.Value;

            SelectedDateText.Text = selectedDate.ToString("dddd, dd MMMM yyyy");

            TaskList.ItemsSource = tasks
                .Where(task => task.DueDate.Date == selectedDate.Date)
                .Select(task => task.Title)
                .ToList();
        }
    }

    public class StudyTask
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }

        public StudyTask(string title, DateTime dueDate)
        {
            Title = title;
            DueDate = dueDate;
        }
    }
}