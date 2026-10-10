namespace StudyProductivityApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Colour { get; set; } = "#808080";

        public List<StudyTask> Tasks { get; set; } = new();
    }
}