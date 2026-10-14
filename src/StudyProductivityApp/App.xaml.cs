using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;

namespace StudyProductivityApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                using var db = new Data.AppDbContext();

                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while initializing the database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
