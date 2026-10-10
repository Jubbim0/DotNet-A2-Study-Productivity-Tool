using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Models;
using System.IO;

namespace StudyProductivityApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<StudyTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StudyProductivityApp"
            );

            Directory.CreateDirectory(folder);

            string databasePath = Path.Combine(folder, "studyapp.db");

            options.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(category => category.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<StudyTask>()
                .Property(task => task.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<StudyTask>()
                .HasOne(task => task.Category)
                .WithMany(category => category.Tasks)
                .HasForeignKey(task => task.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}