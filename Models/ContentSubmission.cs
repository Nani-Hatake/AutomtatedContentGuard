using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomatedContentGuard.Models
{
    public class ContentSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TextContent { get; set; } = string.Empty;

        public double ToxicityScore { get; set; }

        public bool IsFlagged { get; set; }

        public string Status { get; set; } = "Approved";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
