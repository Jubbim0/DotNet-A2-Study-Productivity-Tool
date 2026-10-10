using System.Windows;
using System.Windows.Controls;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;

namespace StudyProductivityApp.Views
{
    public partial class TasksPage : Page
    {
        private readonly TaskService taskService = new();
        private bool isLoaded = false;

        public TasksPage()
        {
            InitializeComponent();

            LoadOptions();
            LoadCategories();

            isLoaded = true;
            LoadTasks();
        }

        // Populate dropdowns
        private void LoadOptions()
        {
            PriorityInput.ItemsSource = Enum.GetValues<TaskPriority>();
            StatusInput.ItemsSource = Enum.GetValues<Models.TaskStatus>();

            PriorityInput.SelectedItem = TaskPriority.Medium;
            StatusInput.SelectedItem = Models.TaskStatus.NotStarted;

            PriorityFilter.Items.Add("All");
            foreach (TaskPriority priority in Enum.GetValues<TaskPriority>())
                PriorityFilter.Items.Add(priority);

            StatusFilter.Items.Add("All");
            foreach (Models.TaskStatus status in Enum.GetValues<Models.TaskStatus>())
                StatusFilter.Items.Add(status);

            SortInput.ItemsSource = new[] { "Deadline", "Priority", "Title" };

            PriorityFilter.SelectedIndex = 0;
            StatusFilter.SelectedIndex = 0;
            SortInput.SelectedIndex = 0;
        }

        // Load categories from SQLite
        private void LoadCategories()
        {
            using AppDbContext db = new();

            var categories = db.Categories
                .OrderBy(category => category.Name)
                .ToList();

            CategoryInput.ItemsSource = categories;

            CategoryFilter.ItemsSource = new[]
            {
                new Category { Id = 0, Name = "All Categories" }
            }.Concat(categories).ToList();

            CategoryFilter.SelectedIndex = 0;
        }

        // Retrieve tasks using the selected filters
        private void LoadTasks()
        {
            int? categoryId = CategoryFilter.SelectedValue as int?;

            if (categoryId == 0)
                categoryId = null;

            TaskPriority? priority =
                PriorityFilter.SelectedItem as TaskPriority?;

            Models.TaskStatus? status =
                StatusFilter.SelectedItem as Models.TaskStatus?;

            string sortBy = SortInput.SelectedItem?.ToString() ?? "Deadline";

            TasksGrid.ItemsSource = taskService.GetFilteredTasks(
                categoryId, priority, status, sortBy);
        }

        // Read task details from the form
        private StudyTask ReadTaskForm()
        {
            return new StudyTask
            {
                Title = TitleInput.Text.Trim(),
                Description = DescriptionInput.Text,
                DueDate = DueDateInput.SelectedDate,
                Priority = (TaskPriority)PriorityInput.SelectedItem,
                Status = (Models.TaskStatus)StatusInput.SelectedItem,
                CategoryId = CategoryInput.SelectedValue as int?
            };
        }

        // Create task
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleInput.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            taskService.AddTask(ReadTaskForm());

            LoadTasks();
            ClearForm();
        }

        // Update selected task
        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not StudyTask selectedTask)
            {
                MessageBox.Show("Please select a task to update.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleInput.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            StudyTask updatedTask = ReadTaskForm();
            updatedTask.Id = selectedTask.Id;

            taskService.UpdateTask(updatedTask);

            LoadTasks();
            ClearForm();
        }

        // Delete selected task
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not StudyTask selectedTask)
            {
                MessageBox.Show("Please select a task to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Delete '{selectedTask.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            taskService.DeleteTask(selectedTask.Id);

            LoadTasks();
            ClearForm();
        }

        // Complete selected task
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not StudyTask selectedTask)
            {
                MessageBox.Show("Please select a task.");
                return;
            }

            taskService.CompleteTask(selectedTask.Id);

            LoadTasks();
            ClearForm();
        }

        // Populate form when a task is selected
        private void TasksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksGrid.SelectedItem is not StudyTask task)
                return;

            TitleInput.Text = task.Title;
            DescriptionInput.Text = task.Description;
            DueDateInput.SelectedDate = task.DueDate;
            PriorityInput.SelectedItem = task.Priority;
            StatusInput.SelectedItem = task.Status;
            CategoryInput.SelectedValue = task.CategoryId;
        }

        // Refresh list when filtering changes
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded)
                return;

            ClearForm();
            LoadTasks();
        }

        // Clear button
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Reset form
        private void ClearForm()
        {
            TasksGrid.SelectedItem = null;

            TitleInput.Clear();
            DescriptionInput.Clear();
            DueDateInput.SelectedDate = null;
            CategoryInput.SelectedIndex = -1;

            PriorityInput.SelectedItem = TaskPriority.Medium;
            StatusInput.SelectedItem = Models.TaskStatus.NotStarted;
        }
    }
}