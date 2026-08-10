namespace AutomatedContentGuard.Models
{
    public class ForbiddenWord
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public int SeverityScore { get; set; } 
    }
}
