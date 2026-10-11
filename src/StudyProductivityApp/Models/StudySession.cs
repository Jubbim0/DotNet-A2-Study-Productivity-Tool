namespace StudyProductivityApp.Models
{
    public class StudySession
    {
        public int Id { get; set; }

        public DateTime CompletedAt { get; set; }

        public int DurationMinutes { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}