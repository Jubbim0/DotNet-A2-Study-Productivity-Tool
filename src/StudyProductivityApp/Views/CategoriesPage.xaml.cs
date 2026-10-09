using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StudyProductivityApp.Models;

namespace StudyProductivityApp.Views
{
    /// <summary>
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        private List<Category> categories = new List<Category>();

        public CategoriesPage()
        {
            InitializeComponent();
        }

    private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = CategoryNameTextBox.Text.Trim();

            Category newCategory = new Category
            {
                Name = categoryName
            };

            categories.Add(newCategory);

            CategoriesListBox.Items.Add(newCategory.Name);

            CategoryNameTextBox.Clear(); 
        }
    }   
}
