using Microsoft.EntityFrameworkCore;
using StudyProductivityApp.Data;
using StudyProductivityApp.Models;

namespace StudyProductivityApp.Services
{
    public class TaskService
    {
        // Create a task
        public void AddTask(StudyTask task)
        {
            using AppDbContext db = new();

            db.Tasks.Add(task);
            db.SaveChanges();
        }

        // Update an existing task
        public void UpdateTask(StudyTask task)
        {
            using AppDbContext db = new();

            StudyTask? existingTask = db.Tasks.Find(task.Id);

            if (existingTask == null)
                return;

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Priority = task.Priority;
            existingTask.Status = task.Status;
            existingTask.CategoryId = task.CategoryId;

            db.SaveChanges();
        }

        // Delete a task
        public void DeleteTask(int taskId)
        {
            using AppDbContext db = new();

            StudyTask? task = db.Tasks.Find(taskId);

            if (task == null)
                return;

            db.Tasks.Remove(task);
            db.SaveChanges();
        }

        // Mark a task as completed
        public void CompleteTask(int taskId)
        {
            using AppDbContext db = new();

            StudyTask? task = db.Tasks.Find(taskId);

            if (task == null)
                return;

            task.Status = Models.TaskStatus.Completed;

            db.SaveChanges();
        }

        // Filter and sort tasks
        public List<StudyTask> GetFilteredTasks(
            int? categoryId,
            TaskPriority? priority,
            Models.TaskStatus? status,
            string sortBy)
        {
            using AppDbContext db = new();

            IQueryable<StudyTask> query = db.Tasks
                .Include(task => task.Category)
                .AsNoTracking();

            // Filter by category
            if (categoryId.HasValue)
                query = query.Where(task => task.CategoryId == categoryId.Value);

            // Filter by priority
            if (priority.HasValue)
                query = query.Where(task => task.Priority == priority.Value);

            // Filter by status
            if (status.HasValue)
                query = query.Where(task => task.Status == status.Value);

            // Sort results
            query = sortBy switch
            {
                "Priority" => query.OrderByDescending(task => task.Priority),
                "Title" => query.OrderBy(task => task.Title),
                _ => query.OrderBy(task => task.DueDate == null)
                          .ThenBy(task => task.DueDate)
            };

            return query.ToList();
        }
    }
}