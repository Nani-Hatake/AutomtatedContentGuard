using System.ComponentModel.DataAnnotations;
namespace AutomatedContentGuard.DTOs
{
    public class CreateContentSubmissionDto
    {
        [Required(ErrorMessage = "TextContent is required.")]
        [StringLength(1000, ErrorMessage = "TextContent cannot exceed 1000 characters.")]
        public string TextContent { get;set; } = string.Empty;
    
    }
}
