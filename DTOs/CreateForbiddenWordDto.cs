using System.ComponentModel.DataAnnotations;
namespace AutomatedContentGuard.DTOs
{
    public class CreateForbiddenWordDto
    {
        [Required(ErrorMessage="Word field is required")]
        public string Word { get; set; } = string.Empty;
        [Range(1,10, ErrorMessage = "SeverityScore must be between 1 and 10.")]
        public int SeverityScore { get; set; }
    }
}
