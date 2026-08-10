using AutomatedContentGuard.Models;
namespace AutomatedContentGuard.Interfaces
{
    public interface IContentRepository
    {
        Task<IEnumerable<ContentSubmission>> GetAllAsync();
        Task<ContentSubmission>GetByIdAsync(int id);
        Task<ContentSubmission> CreateAsync(ContentSubmission contentSubmission);
        Task<bool>DeleteAsync(int id);
        Task<bool> UpdateAsync(ContentSubmission content);
    }
}
