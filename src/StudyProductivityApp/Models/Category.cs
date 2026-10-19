namespace StudyProductivityApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Colour { get; set; } = "#808080";

        // IMPORTANT: Navigation property for tasks associated with this category.
        public List<StudyTask> Tasks { get; set; } = new();
    }
}