using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;
using TaskStatus = StudyProductivityApp.Models.TaskStatus;

namespace StudyProductivityApp.Tests;

[TestFixture]
public class CategoryServiceTests : SqliteTestBase
{
    private CategoryService Service => new(CreateContext);

    [Test]
    public void AddCategory_ValidCategory_AddsCategoryToDatabase()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        // Act
        Service.AddCategory(category);
        // Assert
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        NUnit.Framework.Assert.That(addedCategory.Colour, Is.EqualTo("#FF5733"));
    }

    [Test]
    public void AddCategory_InvalidCategory_ThrowsArgumentException()
    {
        // Arrange
        var category = new Category
        {
            Name = "", // Invalid name
            Colour = "#FF5733"
        };
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Service.AddCategory(category));
    }

    [Test]
    public void UpdateCategory_ExistingCategory_UpdatesCategoryInDatabase()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        // Act
        addedCategory.Name = "Updated Category";
        Service.UpdateCategory(addedCategory);
        // Assert
        context.ChangeTracker.Clear();
        var updatedCategory = context.Categories.FirstOrDefault(c => c.Id == addedCategory.Id);
        Assert.IsNotNull(updatedCategory);
        NUnit.Framework.Assert.That(updatedCategory.Name, Is.EqualTo("Updated Category"));
    }

    [Test]
    public void UpdateCategory_InvalidCategory_ThrowsArgumentException()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        // Act
        addedCategory.Name = ""; // Invalid name
        // Assert
        var ex = Assert.Throws<ArgumentException>(() => Service.UpdateCategory(addedCategory));
    }

    [Test]
    public void UpdateCategory_NonExistingCategory_DoesNothing()
    {
        // Arrange
        var category = new Category
        {
            Id = 999, // Non-existing ID
            Name = "Non-existing Category",
            Colour = "#FF5733"
        };
        // Act
        Service.UpdateCategory(category);
        // Assert
        using var context = CreateContext();
        var nonExistingCategory = context.Categories.FirstOrDefault(c => c.Id == 999);
        Assert.IsNull(nonExistingCategory);
    }

    [Test]
    public void DeleteCategory_ExistingCategory_DeletesCategoryFromDatabase()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        // Act
        Service.DeleteCategory(addedCategory.Id);
        // Assert
        var deletedCategory = context.Categories.FirstOrDefault(c => c.Id == addedCategory.Id);
        Assert.IsNull(deletedCategory);
    }

    [Test]
    public void DeleteCategory_NonExistingCategory_DoesNothing()
    {
        // Act
        Service.DeleteCategory(999); // Non-existing ID
        // Assert
        using var context = CreateContext();
        var nonExistingCategory = context.Categories.FirstOrDefault(c => c.Id == 999);
        Assert.IsNull(nonExistingCategory);
    }

    [Test]
    public void DeleteCategory_CategoryWithTasks_UncategorizesTasks()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted,
            CategoryId = addedCategory.Id
        };
        context.Tasks.Add(task);
        context.SaveChanges();
        // Act
        Service.DeleteCategory(addedCategory.Id);
        // Assert
        context.ChangeTracker.Clear();
        var uncategorizedTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        Assert.IsNotNull(uncategorizedTask);
        Assert.IsNull(uncategorizedTask.CategoryId);
    }

    [Test]
    public void CategoryExists_ExistingCategory_ReturnsTrue()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        // Act
        bool exists = Service.CategoryExists("Test Category");
        // Assert
        Assert.IsTrue(exists);
    }

    [Test]
    public void CategoryExists_NonExistingCategory_ReturnsFalse()
    {
        // Act
        bool exists = Service.CategoryExists("Non-existing Category");
        // Assert
        Assert.IsFalse(exists);
    }

    [Test]
    public void CategoryExists_ExistingCategoryWithExclusion_ReturnsFalse()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        // Act
        bool exists = Service.CategoryExists("Test Category", addedCategory.Id);
        // Assert
        Assert.IsFalse(exists);
    }

    [Test]
    public void GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var category1 = new Category
        {
            Name = "Category 1",
            Colour = "#FF5733"
        };
        var category2 = new Category
        {
            Name = "Category 2",
            Colour = "#33FF57"
        };
        Service.AddCategory(category1);
        Service.AddCategory(category2);
        // Act
        var categories = Service.GetCategories();
        // Assert
        NUnit.Framework.Assert.That(categories.Count, Is.EqualTo(2));
        Assert.IsTrue(categories.Any(c => c.Name == "Category 1"));
        Assert.IsTrue(categories.Any(c => c.Name == "Category 2"));
    }

    [Test]
    public void GetTasksForCategory_ExistingCategory_ReturnsTasks()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        var task1 = new StudyTask
        {
            Title = "Task 1",
            Description = "This is task 1.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted,
            CategoryId = addedCategory.Id
        };
        var task2 = new StudyTask
        {
            Title = "Task 2",
            Description = "This is task 2.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.NotStarted,
            CategoryId = addedCategory.Id
        };
        context.Tasks.Add(task1);
        context.Tasks.Add(task2);
        context.SaveChanges();
        // Act
        var tasks = Service.GetTasksForCategory(addedCategory.Id);
        // Assert
        NUnit.Framework.Assert.That(tasks.Count(), Is.EqualTo(2));
        Assert.IsTrue(tasks.Any(t => t.Title == "Task 1"));
        Assert.IsTrue(tasks.Any(t => t.Title == "Task 2"));
    }

    [Test]
    public void GetTasksForCategory_NonExistingCategory_ReturnsEmpty()
    {
        // Act
        var tasks = Service.GetTasksForCategory(999); // Non-existing category ID
        // Assert
        Assert.IsEmpty(tasks);
    }

    [Test]
    public void AssignTaskToCategory_ValidTaskAndCategory_AssignsTaskToCategory()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        context.Tasks.Add(task);
        context.SaveChanges();
        // Act
        Service.AssignTaskToCategory(task.Id, addedCategory.Id);
        // Assert
        context.ChangeTracker.Clear();
        var assignedTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        Assert.IsNotNull(assignedTask);
        NUnit.Framework.Assert.That(assignedTask.CategoryId, Is.EqualTo(addedCategory.Id));
    }

    [Test]
    public void AssignTaskToCategory_NonExistingTask_ThrowsArgumentException()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        // Act
        Assert.Throws<ArgumentException>(() => Service.AssignTaskToCategory(999, addedCategory.Id));
        // Assert
        var nonExistingTask = context.Tasks.FirstOrDefault(t => t.Id == 999);
        Assert.IsNull(nonExistingTask);
    }

    [Test]
    public void AssignTaskToCategory_NonExistingCategory_ThrowsArgumentException()
    {
        // Arrange
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        using var context = CreateContext();
        context.Tasks.Add(task);
        context.SaveChanges();
        // Act
        Assert.Throws<ArgumentException>(() => Service.AssignTaskToCategory(task.Id, 999));
        // Assert
        var unassignedTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        Assert.IsNotNull(unassignedTask);
        Assert.IsNull(unassignedTask.CategoryId);
    }

    [Test]
    public void AssignTaskToCategory_TaskAlreadyAssigned_ThrowsArgumentException()
    {
        // Arrange
        var category1 = new Category
        {
            Name = "Category 1",
            Colour = "#FF5733"
        };
        var category2 = new Category
        {
            Name = "Category 2",
            Colour = "#33FF57"
        };
        Service.AddCategory(category1);
        Service.AddCategory(category2);
        using var context = CreateContext();
        var addedCategory1 = context.Categories.FirstOrDefault(c => c.Name == "Category 1");
        var addedCategory2 = context.Categories.FirstOrDefault(c => c.Name == "Category 2");
        Assert.IsNotNull(addedCategory1);
        Assert.IsNotNull(addedCategory2);
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted,
            CategoryId = addedCategory1.Id
        };
        context.Tasks.Add(task);
        context.SaveChanges();
        // Act
        Assert.Throws<ArgumentException>(() => Service.AssignTaskToCategory(task.Id, addedCategory2.Id));
        // Assert
        var updatedTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        Assert.IsNotNull(updatedTask);
        NUnit.Framework.Assert.That(updatedTask.CategoryId, Is.EqualTo(addedCategory1.Id));
    }

    [Test]
    public void AssignTaskToCategory_Invalid_ThrowsArgumentException()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Service.AssignTaskToCategory(0, addedCategory.Id));
        ex = Assert.Throws<ArgumentException>(() => Service.AssignTaskToCategory(1, 0));

    }

    [Test]
    public void RemoveTaskFromCategory_ExistingTask_UnassignsTaskFromCategory()
    {
        // Arrange
        var category = new Category
        {
            Name = "Test Category",
            Colour = "#FF5733"
        };
        Service.AddCategory(category);
        using var context = CreateContext();
        var addedCategory = context.Categories.FirstOrDefault(c => c.Name == "Test Category");
        Assert.IsNotNull(addedCategory);
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted,
            CategoryId = addedCategory.Id
        };
        context.Tasks.Add(task);
        context.SaveChanges();
        // Act
        Service.RemoveTaskFromCategory(task.Id, addedCategory.Id);
        // Assert
        context.ChangeTracker.Clear();
        var unassignedTask = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        Assert.IsNotNull(unassignedTask);
        Assert.IsNull(unassignedTask.CategoryId);
    }

    [Test]
    public void RemoveTaskFromCategory_NonExistingTask_ThrowsArgumentException()
    {
        // Act
        Assert.Throws<ArgumentException>(() => Service.RemoveTaskFromCategory(999, 1));
        // Assert
        using var context = CreateContext();
        var nonExistingTask = context.Tasks.FirstOrDefault(t => t.Id == 999);
        Assert.IsNull(nonExistingTask);
    }


}