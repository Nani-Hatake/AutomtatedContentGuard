using AutomatedContentGuard.Data;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;
using Microsoft.EntityFrameworkCore;

namespace AutomatedContentGuard.Repositories
{
    public class ForbiddenWordRepo : IForbiddenWordRepository
    {
        private readonly ApplicationDbContext _context;

        public ForbiddenWordRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ForbiddenWord>> GetAllAsync()
        {
            return await _context.ForbiddenWords.ToListAsync();
        }

        public async Task<ForbiddenWord?> GetByIdAsync(int id)
        {
            return await _context.ForbiddenWords.FindAsync(id);
        }

        public async Task<ForbiddenWord> AddAsync(ForbiddenWord word)
        {
            await _context.ForbiddenWords.AddAsync(word);
            await _context.SaveChangesAsync();
            return word;
        }

        public async Task<bool> UpdateAsync(ForbiddenWord word)
        {
            var existingWord = await _context.ForbiddenWords.FindAsync(word.Id);
            if (existingWord == null) return false;

            existingWord.Word = word.Word;
            existingWord.SeverityScore = word.SeverityScore;

            _context.ForbiddenWords.Update(existingWord);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var word = await _context.ForbiddenWords.FindAsync(id);
            if (word == null) return false;

            _context.ForbiddenWords.Remove(word);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<ForbiddenWord> CreateAysnc(ForbiddenWord forbiddenWord)
        {
            throw new NotImplementedException();
        }
    }
}