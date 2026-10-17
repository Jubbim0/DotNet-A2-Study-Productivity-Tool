using StudyProductivityApp.Models;

namespace StudyProductivityApp.Interfaces
{
    public interface ICategoryService
    {
        void AddCategory(Category category);

        void UpdateCategory(Category category);

        void DeleteCategory(int categoryId);

        bool CategoryExists(string name, int? excludeCategoryId = null);

        List<Category> GetCategories();

        List<StudyTask> GetTasksForCategory(int categoryId);

        List<StudyTask> GetUncategorisedTasks();

        void AssignTaskToCategory(int taskId, int categoryId);

        void RemoveTaskFromCategory(int taskId, int categoryId);
    }
}
