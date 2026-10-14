using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;

namespace StudyProductivityApp.Services
{
    public class CategoryService
    {
        private static void ValidateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be empty.");
            if (category.Colour == null || !category.Colour.StartsWith("#") || category.Colour.Length != 7)
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

            existingCategory.Name = category.Name;
            existingCategory.Colour = category.Colour;
            existingCategory.Tasks = category.Tasks;


            db.SaveChanges();
        }

        // delete a category. If the category has tasks, they will be uncategorized (CategoryId set to null).
        public void DeleteCategory(int categoryId)
        {
            using AppDbContext db = new();

            Category? category = db.Categories.Find(categoryId);

            foreach(var task in db.Tasks.Where(t => t.CategoryId == categoryId))
            {
                task.CategoryId = null;
                task.Category = null;
            }

            if (category == null)
                return;

            db.Categories.Remove(category);

            db.SaveChanges();
        }

        // Check if a category exists by its name, excluding a specific category ID (useful for updates).
        public bool CategoryExists(string name, int? excludeCategoryId = null)
        {
            using AppDbContext db = new();

            return db.Categories.Any(c => c.Name == name && (!excludeCategoryId.HasValue || c.Id != excludeCategoryId.Value));
        }
    }
}