using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;
using StudyProductivityApp.Services;
using TaskStatus = StudyProductivityApp.Models.TaskStatus;

namespace StudyProductivityApp.Tests;

[TestFixture]
public class TaskServiceTests : SqliteTestBase
{
    private TaskService Service => new(CreateContext);

    [Test]
    public void AddTask_ValidTask_AddsTaskToDatabase()
    {
        // Arrange
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        // Act
        Service.AddTask(task);
        // Assert
        using var context = CreateContext();
        var addedTask = context.Tasks.FirstOrDefault(t => t.Title == "Test Task");
        Assert.IsNotNull(addedTask);
        NUnit.Framework.Assert.That(addedTask.Description, Is.EqualTo("This is a test task."));
    }

    [Test]
    public void UpdateTask_ExistingTask_UpdatesTaskInDatabase()
    {
        // Arrange
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        Service.AddTask(task);
        using var context = CreateContext();
        var addedTask = context.Tasks.FirstOrDefault(t => t.Title == "Test Task");
        Assert.IsNotNull(addedTask);
        // Act
        addedTask.Description = "Updated description.";
        Service.UpdateTask(addedTask);
        // Assert
        context.ChangeTracker.Clear();
        var updatedTask = context.Tasks.FirstOrDefault(t => t.Id == addedTask.Id);
        Assert.IsNotNull(updatedTask);
        NUnit.Framework.Assert.That(updatedTask.Description, Is.EqualTo("Updated description."));
    }

    [Test]
    public void DeleteTask_ExistingTask_DeletesTaskFromDatabase()
    {
        // Arrange
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        Service.AddTask(task);
        using var context = CreateContext();
        var addedTask = context.Tasks.FirstOrDefault(t => t.Title == "Test Task");
        Assert.IsNotNull(addedTask);
        // Act
        Service.DeleteTask(addedTask.Id);
        // Assert
        var deletedTask = context.Tasks.FirstOrDefault(t => t.Id == addedTask.Id);
        Assert.IsNull(deletedTask);
    }

    [Test]
    public void CompleteTask_ExistingTask_MarksTaskAsCompleted()
    {
        // Arrange
        var task = new StudyTask
        {
            Title = "Test Task",
            Description = "This is a test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        Service.AddTask(task);
        using var context = CreateContext();
        var addedTask = context.Tasks.FirstOrDefault(t => t.Title == "Test Task");
        Assert.IsNotNull(addedTask);
        // Act
        Service.CompleteTask(addedTask.Id);
        // Assert
        context.ChangeTracker.Clear();
        var completedTask = context.Tasks.FirstOrDefault(t => t.Id == addedTask.Id);
        Assert.IsNotNull(completedTask);
        NUnit.Framework.Assert.That(completedTask.Status, Is.EqualTo(TaskStatus.Completed));
    }

    [Test]
    public void GetFilteredTasks_ReturnsAllTasksInDatabase()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.NotStarted
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var tasks = Service.GetFilteredTasks(null, null, null, "Deadline");
        // Assert
        NUnit.Framework.Assert.That(tasks.Count(), Is.EqualTo(2));
    }

    [Test]
    public void GetFilteredTasks_SingleFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Completed
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var filteredTasks = Service.GetFilteredTasks(null, null, TaskStatus.Completed, "Deadline");
        // Assert
        NUnit.Framework.Assert.That(filteredTasks.Count(), Is.EqualTo(1));
        NUnit.Framework.Assert.That(filteredTasks.First().Title, Is.EqualTo("Test Task 2"));
    }

    [Test]
    public void GetFilteredTasks_MultipleFilters_ReturnsFilteredTasks()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted,
            Priority = TaskPriority.Low
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Completed,
            Priority = TaskPriority.High
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var filteredTasks = Service.GetFilteredTasks(null, TaskPriority.High, TaskStatus.Completed, "Deadline");
        // Assert
        NUnit.Framework.Assert.That(filteredTasks.Count(), Is.EqualTo(1));
        NUnit.Framework.Assert.That(filteredTasks.First().Title, Is.EqualTo("Test Task 2"));
    }

    [Test]
    public void GetFilteredTasks_NoMatchingFilters_ReturnsEmpty()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted,
            Priority = TaskPriority.High
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Completed,
            Priority = TaskPriority.Low
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var filteredTasks = Service.GetFilteredTasks(null, TaskPriority.High, TaskStatus.Completed, "Deadline");
        // Assert
        NUnit.Framework.Assert.That(filteredTasks.Count(), Is.EqualTo(0));
    }

    [Test]
    public void GetFilteredTasks_NoFilters_ReturnsAllTasks()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.NotStarted
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Completed
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var filteredTasks = Service.GetFilteredTasks(null, null, null, "Deadline");
        // Assert
        NUnit.Framework.Assert.That(filteredTasks.Count(), Is.EqualTo(2));
    }

    [Test]
    public void GetFilteredTasks_SortByDeadline_ReturnsTasksSortedByDeadline()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.NotStarted
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.Completed
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var filteredTasks = Service.GetFilteredTasks(null, null, null, "Deadline");
        // Assert
        NUnit.Framework.Assert.That(filteredTasks.Count(), Is.EqualTo(2));
        NUnit.Framework.Assert.That(filteredTasks[0].Title, Is.EqualTo("Test Task 2"));
        NUnit.Framework.Assert.That(filteredTasks[1].Title, Is.EqualTo("Test Task 1"));
    }

    [Test]
    public void GetFilteredTasks_SortByPriority_HighFirst()
    {
        // Arrange
        var task1 = new StudyTask
        {
            Title = "Test Task 1",
            Description = "This is the first test task.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.NotStarted,
            Priority = TaskPriority.Low
        };
        var task2 = new StudyTask
        {
            Title = "Test Task 2",
            Description = "This is the second test task.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.Completed,
            Priority = TaskPriority.High
        };
        Service.AddTask(task1);
        Service.AddTask(task2);
        // Act
        var filteredTasks = Service.GetFilteredTasks(null, null, null, "Priority");
        // Assert
        NUnit.Framework.Assert.That(filteredTasks.Count(), Is.EqualTo(2));
        NUnit.Framework.Assert.That(filteredTasks[0].Title, Is.EqualTo("Test Task 2"));
        NUnit.Framework.Assert.That(filteredTasks[1].Title, Is.EqualTo("Test Task 1"));
    }

}