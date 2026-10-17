using StudyProductivityApp.Models;

namespace StudyProductivityApp.Interfaces
{
    public interface ITaskService
    {
        void AddTask(StudyTask task);

        void UpdateTask(StudyTask task);

        void DeleteTask(int taskId);

        void CompleteTask(int taskId);

        List<StudyTask> GetFilteredTasks(
            int? categoryId,
            TaskPriority? priority,
            Models.TaskStatus? status,
            string sortBy);
    }
}