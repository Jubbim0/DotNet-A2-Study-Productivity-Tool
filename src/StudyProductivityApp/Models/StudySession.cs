namespace StudyProductivityApp.Models
{
    public class StudySession
    {
        public int Id { get; set; }

        public DateTime CompletedAt { get; set; }

        public int DurationMinutes { get; set; }

        // Optional category relationship for organising study sessions.
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}