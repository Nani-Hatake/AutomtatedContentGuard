using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Models;

namespace AutomatedContentGuard.Interfaces
{
    public interface IForbiddenWordService
    {
        Task<IEnumerable<ForbiddenWord>> GetAllAsync();
        Task<ForbiddenWord?> GetByIdAsync(int id);
        Task<ForbiddenWord> CreateAsync(CreateForbiddenWordDto dto);
        Task<bool> DeleteAsync(int id);
    }
}