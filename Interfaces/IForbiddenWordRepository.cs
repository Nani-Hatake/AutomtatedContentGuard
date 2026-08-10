using AutomatedContentGuard.Models;

namespace AutomatedContentGuard.Interfaces
{
    public interface IForbiddenWordRepository
    {
        Task<IEnumerable<ForbiddenWord>> GetAllAsync();
        Task<ForbiddenWord> GetByIdAsync(int id);
        Task<ForbiddenWord> CreateAysnc(ForbiddenWord dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(ForbiddenWord forbiddenWord);
        Task<ForbiddenWord> AddAsync(ForbiddenWord word);
    }
}
