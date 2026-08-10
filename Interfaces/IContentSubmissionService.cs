using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Models;

namespace AutomatedContentGuard.Interfaces
{
    public interface IContentSubmissionService
    {
        Task<IEnumerable<ContentSubmission>> GetAllAsync();
        Task<ContentSubmission?> GetByIdAsync(int id);
        Task<ContentSubmission> CreateAsync(CreateContentSubmissionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}