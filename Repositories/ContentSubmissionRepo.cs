using System.Collections.Generic;
using System.Threading.Tasks;
using AutomatedContentGuard.Data;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;
using Microsoft.EntityFrameworkCore;

namespace AutomatedContentGuard.Repositories
{
    public class ContentSubmissionRepo : IContentRepository
    {
        private readonly ApplicationDbContext _context;

        public ContentSubmissionRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContentSubmission>> GetAllAsync()
        {
            return await _context.ContentSubmissions.ToListAsync();
        }

        public async Task<ContentSubmission?> GetByIdAsync(int id)
        {
            return await _context.ContentSubmissions.FindAsync(id);
        }

        public async Task<ContentSubmission> CreateAsync(ContentSubmission submission)
        {
            await _context.ContentSubmissions.AddAsync(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<ContentSubmission> AddAsync(ContentSubmission submission)
        {
            await _context.ContentSubmissions.AddAsync(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<ContentSubmission> UpdateAsync(ContentSubmission submission)
        {
            _context.ContentSubmissions.Update(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var submission = await _context.ContentSubmissions.FindAsync(id);
            if (submission == null)
            {
                return false;
            }

            _context.ContentSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
