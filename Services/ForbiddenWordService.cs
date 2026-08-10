using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;

namespace AutomatedContentGuard.Services
{
    public class ForbiddenWordService : IForbiddenWordService
    {
        private readonly IForbiddenWordRepository _forbiddenWordRepo;

        public ForbiddenWordService(IForbiddenWordRepository forbiddenWordRepo)
        {
            _forbiddenWordRepo = forbiddenWordRepo;
        }

        public async Task<IEnumerable<ForbiddenWord>> GetAllAsync()
        {
            return await _forbiddenWordRepo.GetAllAsync();
        }

        public async Task<ForbiddenWord?> GetByIdAsync(int id)
        {
            return await _forbiddenWordRepo.GetByIdAsync(id);
        }

        public async Task<ForbiddenWord> CreateAsync(CreateForbiddenWordDto dto)
        {
            var word = new ForbiddenWord
            {
                Word = dto.Word,
                SeverityScore = dto.SeverityScore
            };

            return await _forbiddenWordRepo.AddAsync(word);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _forbiddenWordRepo.DeleteAsync(id);
        }
    }
}