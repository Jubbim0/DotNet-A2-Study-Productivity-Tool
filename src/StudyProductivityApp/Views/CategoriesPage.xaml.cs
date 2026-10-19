using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;

namespace StudyProductivityApp.Views
{
    /// <summary>
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        private readonly CategoryService categoryService = new();
        private bool isRefreshing;

        public CategoriesPage()
        {
            InitializeComponent();
            RefreshCategories();
        }

        // Reload categories and preserve the selected category when possible.
        private void RefreshCategories(int? selectedCategoryId = null)
        {
            isRefreshing = true;

            List<Category> categories = categoryService.GetCategories();

            CategoriesGrid.ItemsSource = categories;
            CategoriesGrid.SelectedItem = categories.FirstOrDefault(category =>
                category.Id == selectedCategoryId);
            isRefreshing = false;

            CategoriesEmptyText.Visibility = categories.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            RefreshTaskLists();
        }

        // Display tasks assigned to the selected category and those available for assignment.
        private void RefreshTaskLists()
        {
            if (CategoriesGrid.SelectedItem is Category selectedCategory)
            {
                AssociatedTasksGrid.ItemsSource = categoryService.GetTasksForCategory(selectedCategory.Id);
                AvailableTasksGrid.ItemsSource = categoryService.GetUncategorisedTasks();
                return;
            }
            else
            {
                AssociatedTasksGrid.ItemsSource = null;
                AvailableTasksGrid.ItemsSource = null;
            }

            AssociatedTasksEmptyText.Text = CategoriesGrid.SelectedItem is Category
                ? "No tasks are assigned to this category."
                : "Select a category to see its tasks.";
            AssociatedTasksEmptyText.Visibility = AssociatedTasksGrid.Items.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            AvailableTasksEmptyText.Text = CategoriesGrid.SelectedItem is Category
                ? "No unassigned tasks are available."
                : "Select a category to assign tasks.";
            AvailableTasksEmptyText.Visibility = AvailableTasksGrid.Items.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            UpdateActionStates();
        }

        // Enable or disable actions based on the current selections.
        private void UpdateActionStates()
        {
            bool hasCategory = CategoriesGrid.SelectedItem is Category;
            UpdateCategoryButton.IsEnabled = hasCategory;
            DeleteCategoryButton.IsEnabled = hasCategory;
            AddTaskButton.IsEnabled = hasCategory && AvailableTasksGrid.SelectedItem is StudyTask;
            RemoveTaskButton.IsEnabled = hasCategory && AssociatedTasksGrid.SelectedItem is StudyTask;
        }

        // Validate the hexadecimal colour and update its preview.
        private void CategoryColourInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string colour = CategoryColourInput.Text.Trim();
            if (Regex.IsMatch(colour, "^#[0-9A-Fa-f]{6}$"))
            {
                CategoryColourPreview.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(colour));
                CategoryColourPreview.ToolTip = colour;
            }
            else
            {
                CategoryColourPreview.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                CategoryColourPreview.ToolTip = "Enter a valid colour in #RRGGBB format.";
            }
        }

        // Validate and create a new category.
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetCategoryInput(out Category category))
                return;

            try
            {
                if (categoryService.CategoryExists(category.Name))
                {
                    MessageBox.Show("A category with this name already exists.", "Category Exists",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                categoryService.AddCategory(category);
                RefreshCategories(category.Id);
            }
            catch (ArgumentException exception)
            {
                ShowError(exception.Message);
            }
            catch (Exception exception)
            {
                ShowError($"The category could not be added. {exception.Message}");
            }
        }

        // Update the selected category after validating its details.
        private void UpdateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Select a category to update.", "No Category Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryGetCategoryInput(out Category category))
                return;

            category.Id = selectedCategory.Id;

            try
            {
                if (categoryService.CategoryExists(category.Name, category.Id))
                {
                    MessageBox.Show("A category with this name already exists.", "Category Exists",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                categoryService.UpdateCategory(category);
                RefreshCategories(category.Id);
            }
            catch (ArgumentException exception)
            {
                ShowError(exception.Message);
            }
            catch (Exception exception)
            {
                ShowError($"The category could not be updated. {exception.Message}");
            }
        }

        // Confirm deletion before removing the selected category.
        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Select a category to delete.", "No Category Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Delete '{selectedCategory.Name}'? Its tasks will remain but become uncategorized.",
                "Delete Category",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                categoryService.DeleteCategory(selectedCategory.Id);
                ClearForm();
                RefreshCategories();
            }
            catch (Exception exception)
            {
                ShowError($"The category could not be deleted. {exception.Message}");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Validate category input before creating or updating a category.
        private bool TryGetCategoryInput(out Category category)
        {
            string name = CategoryNameInput.Text.Trim();
            string colour = CategoryColourInput.Text.Trim();
            category = new Category { Name = name, Colour = colour };

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Enter a category name.", "Invalid Category",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (name.Length > 100)
            {
                MessageBox.Show("Category names cannot exceed 100 characters.", "Invalid Category",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            isRefreshing = true;
            CategoriesGrid.SelectedItem = null;
            CategoryNameInput.Clear();
            CategoryColourInput.Text = "#808080";
            AssociatedTasksGrid.ItemsSource = null;
            AvailableTasksGrid.ItemsSource = null;
            isRefreshing = false;
            UpdateActionStates();
            AssociatedTasksEmptyText.Text = "Select a category to see its tasks.";
            AssociatedTasksEmptyText.Visibility = Visibility.Visible;
            AvailableTasksEmptyText.Text = "Select a category to assign tasks.";
            AvailableTasksEmptyText.Visibility = Visibility.Visible;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Category Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CategoriesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isRefreshing)
                return;

            if (CategoriesGrid.SelectedItem is Category selectedCategory)
            {
                CategoryNameInput.Text = selectedCategory.Name;
                CategoryColourInput.Text = selectedCategory.Colour;
            }

            RefreshTaskLists();
        }

        private void TaskGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateActionStates();
        }

        // Assign the selected unassigned task to the chosen category.
        private void AddTaskToCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Select a category before adding a task.", "No Category Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AvailableTasksGrid.SelectedItem is not StudyTask selectedTask)
            {
                MessageBox.Show("Select an available task to add.", "No Task Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                categoryService.AssignTaskToCategory(selectedTask.Id, selectedCategory.Id);
                RefreshCategories(selectedCategory.Id);
            }
            catch (Exception exception)
            {
                ShowError($"The task could not be added to the category. {exception.Message}");
            }
        }

        // Remove the selected task's association with the category.
        private void RemoveTaskFromCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Select a category before removing a task.", "No Category Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AssociatedTasksGrid.SelectedItem is not StudyTask selectedTask)
            {
                MessageBox.Show("Select a task to remove from the category.", "No Task Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                categoryService.RemoveTaskFromCategory(selectedTask.Id, selectedCategory.Id);
                RefreshCategories(selectedCategory.Id);
            }
            catch (Exception exception)
            {
                ShowError($"The task could not be removed from the category. {exception.Message}");
            }
        }
    }
}
