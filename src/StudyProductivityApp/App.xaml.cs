using System.Windows;
using Microsoft.EntityFrameworkCore;

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

                // Apply any pending database migrations on startup.
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                // this section will displays the error and close the application if initialization fails.

                MessageBox.Show($"An error occurred while initializing the database: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
