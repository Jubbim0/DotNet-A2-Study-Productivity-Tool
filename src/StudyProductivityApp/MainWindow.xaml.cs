using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StudyProductivityApp.Views;

namespace StudyProductivityApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetActiveNavigation(CategoriesNavigationButton);
            MainFrame.Navigate(new CategoriesPage());
        }

        private void SetActiveNavigation(Button selected)
        {
            Button[] navigationButtons =
            {
                CategoriesNavigationButton,
                TasksNavigationButton,
                FocusNavigationButton,
                CalendarNavigationButton,
                StatisticsNavigationButton
            };

            foreach (Button button in navigationButtons)
            {
                bool isSelected = button == selected;
                if (isSelected)
                {
                    button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DBEAFE"));
                    button.Foreground = (Brush)FindResource("PrimaryBrush");
                    button.FontWeight = FontWeights.SemiBold;
                    button.BorderBrush = (Brush)FindResource("PrimaryBrush");
                    button.BorderThickness = new Thickness(3, 0, 0, 0);
                }
                else
                {
                    button.ClearValue(Button.BackgroundProperty);
                    button.ClearValue(Button.ForegroundProperty);
                    button.ClearValue(Button.FontWeightProperty);
                    button.ClearValue(Button.BorderBrushProperty);
                    button.ClearValue(Button.BorderThicknessProperty);
                }
            }
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavigation(CategoriesNavigationButton);
            MainFrame.Navigate(new CategoriesPage());
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavigation(TasksNavigationButton);
            MainFrame.Navigate(new TasksPage());
        }

        private void Focus_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavigation(FocusNavigationButton);
            MainFrame.Navigate(new FocusPage());
        }

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavigation(CalendarNavigationButton);
            MainFrame.Navigate(new CalendarPage());
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavigation(StatisticsNavigationButton);
            MainFrame.Navigate(new StatisticsPage());
        }
    }
}