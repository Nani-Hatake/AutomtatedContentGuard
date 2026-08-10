namespace AutomatedContentGuard.Models
{
    public class ContentSubmission
    {
        public int Id { get; set; }
        public string TextContent { get; set; } = string.Empty;
        public int ToxicityScore { get; set; }
        public string Status { get; set; }= "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsFlagged { get; internal set; }
        public DateTime SubmittedAt { get; internal set; }
    }
}
