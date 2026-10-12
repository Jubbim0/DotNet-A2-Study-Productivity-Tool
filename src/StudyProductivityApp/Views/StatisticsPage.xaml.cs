using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using StudyProductivityApp.Services;

namespace StudyProductivityApp.Views
{
    public partial class StatisticsPage : Page
    {
        private readonly StatisticsService statisticsService = new();

        public StatisticsPage()
        {
            InitializeComponent();

            LoadStatistics();
            LoadCategoryChart();
            LoadDailyChart();
        }

        // Display numerical statistics
        private void LoadStatistics()
        {
            int totalMinutes = statisticsService.GetTotalStudyMinutes();

            TotalStudyTimeText.Text = $"{totalMinutes / 60}h {totalMinutes % 60}m";

            CompletedSessionsText.Text =
                statisticsService.GetCompletedSessions().ToString();

            AverageDurationText.Text =
                $"{statisticsService.GetAverageSessionMinutes():F1} min";

            TaskCompletionText.Text =
                $"{statisticsService.GetTaskCompletionRate():F1}%";
        }

        // Display study time by category
        private void LoadCategoryChart()
        {
            var data = statisticsService.GetStudyTimeByCategory()
                .OrderByDescending(item => item.Value)
                .ToList();

            CategoryChart.Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Study Minutes",
                    Values = data.Select(item => item.Value).ToArray()
                }
            };

            CategoryChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = data.Select(item => item.Key).ToArray()
                }
            };

            CategoryChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Minutes",
                    MinLimit = 0
                }
            };
        }

        // Display daily study activity
        private void LoadDailyChart()
        {
            var data = statisticsService.GetDailyStudyTime();

            if (data.Count == 0)
                return;

            DateTime firstDate = data.Keys.Min();
            DateTime lastDate = data.Keys.Max();

            var dates = Enumerable.Range(0, (lastDate - firstDate).Days + 1)
                .Select(day => firstDate.AddDays(day))
                .ToList();

            DailyChart.Series = new ISeries[]
            {
                new LineSeries<int>
                {
                    Name = "Study Minutes",
                    Values = dates.Select(date =>
                        data.GetValueOrDefault(date, 0)).ToArray()
                }
            };

            DailyChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = dates.Select(date =>
                        date.ToString("dd/MM")).ToArray()
                }
            };

            DailyChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Minutes",
                    MinLimit = 0
                }
            };
        }
    }
}