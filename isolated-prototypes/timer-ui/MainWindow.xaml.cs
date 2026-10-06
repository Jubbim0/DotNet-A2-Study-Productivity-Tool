using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FocusTimerUIPrototype
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer;
        private int remainingSeconds;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += Timer_Tick;
        }

        private void StartSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DurationComboBox.SelectedItem is not ComboBoxItem durationItem)
            {
                MessageBox.Show("Please select a duration.");
                return;
            }

            string duration = durationItem.Content.ToString()!;
            string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "None";
            string task = (TaskComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "None";

            remainingSeconds = duration switch
            {
                "25 minutes" => 25 * 60,
                "30 minutes" => 30 * 60,
                "45 minutes" => 45 * 60,
                "60 minutes" => 60 * 60,
                _ => 25 * 60
            };

            SessionCategoryText.Text = category;
            SessionTaskText.Text = task;

            SetupPanel.Visibility = Visibility.Collapsed;
            TimerPanel.Visibility = Visibility.Visible;

            TimerText.Text = $"{remainingSeconds / 60:00}:{remainingSeconds % 60:00}";
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            remainingSeconds--;

            TimerText.Text = $"{remainingSeconds / 60:00}:{remainingSeconds % 60:00}";

            if (remainingSeconds <= 0)
            {
                timer.Stop();
                MessageBox.Show("Focus session complete!");
            }
        }

        private void EndSessionButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            TimerPanel.Visibility = Visibility.Collapsed;
            SetupPanel.Visibility = Visibility.Visible;
        }
    }
}