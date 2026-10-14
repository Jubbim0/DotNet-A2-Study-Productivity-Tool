using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using System.Text.RegularExpressions;

namespace StudyProductivityApp.Services
{
    public class CategoryService
    {
        private static void ValidateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be empty.");
            category.Name = category.Name.Trim();
            if (string.IsNullOrEmpty(category.Colour) || !Regex.IsMatch(category.Colour, "^#[0-9A-Fa-f]{6}$"))
                throw new ArgumentException("Invalid colour format. Use hex format like #RRGGBB.");
            if (category.Name.Length > 100)
                throw new ArgumentException("Category name cannot exceed 100 characters.");
        }
        // Create a new category. Throws ArgumentException if the category is invalid.
        public void AddCategory(Category category)
        {
            using AppDbContext db = new();

            ValidateCategory(category);

            db.Categories.Add(category);

            db.SaveChanges();
        }

        // Update an existing category. Throws ArgumentException if the category is invalid.
        public void UpdateCategory(Category category)
        {
            using AppDbContext db = new();
            ValidateCategory(category);

            Category? existingCategory = db.Categories.Find(category.Id);

            if (existingCategory == null)
                return;

            existingCategory.Name = category.Name.Trim();
            existingCategory.Colour = category.Colour;

            db.SaveChanges();
        }

        // delete a category. If the category has tasks, they will be uncategorized (CategoryId set to null).
        public void DeleteCategory(int categoryId)
        {
            using AppDbContext db = new();

            Category? category = db.Categories.Find(categoryId);

            if (category == null)
                return;

            foreach (var task in db.Tasks.Where(task => task.CategoryId == categoryId))
            {
                task.CategoryId = null;
                task.Category = null;
            }

            db.Categories.Remove(category);

            db.SaveChanges();
        }

        // Check if a category exists by its name, excluding a specific category ID (useful for updates).
        public bool CategoryExists(string name, int? excludeCategoryId = null)
        {
            using AppDbContext db = new();

            string normalizedName = name.Trim();

            return db.Categories.Any(category => category.Name == normalizedName &&
                (!excludeCategoryId.HasValue || category.Id != excludeCategoryId.Value));
        }

        public List<Category> GetCategories()
        {
            using AppDbContext db = new();

            return db.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToList();
        }

        public List<StudyTask> GetTasksForCategory(int categoryId)
        {
            using AppDbContext db = new();

            return db.Tasks
                .AsNoTracking()
                .Where(task => task.CategoryId == categoryId)
                .OrderBy(task => task.Title)
                .ToList();
        }

        public List<StudyTask> GetUncategorizedTasks()
        {
            using AppDbContext db = new();

            return db.Tasks
                .AsNoTracking()
                .Where(task => task.CategoryId == null)
                .OrderBy(task => task.Title)
                .ToList();
        }

        public void AssignTaskToCategory(int taskId, int categoryId)
        {
            using AppDbContext db = new();

            StudyTask? task = db.Tasks.Find(taskId);
            bool categoryExists = db.Categories.Any(category => category.Id == categoryId);

            if (task == null)
                throw new ArgumentException("The selected task no longer exists.");
            if (!categoryExists)
                throw new ArgumentException("The selected category no longer exists.");
            if (task.CategoryId.HasValue)
                throw new ArgumentException("The selected task is already assigned to a category.");

            task.CategoryId = categoryId;
            db.SaveChanges();
        }

        public void RemoveTaskFromCategory(int taskId, int categoryId)
        {
            using AppDbContext db = new();

            StudyTask? task = db.Tasks.Find(taskId);

            if (task == null || task.CategoryId != categoryId)
                throw new ArgumentException("The selected task is no longer in this category.");

            task.CategoryId = null;
            db.SaveChanges();
        }
    }
}