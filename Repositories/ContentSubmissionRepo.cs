using AutomatedContentGuard.Models;
using AutomatedContentGuard.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutomatedContentGuard.Data;
namespace AutomatedContentGuard.Repositories
{
    public class ContentSubmissionRepo:IContentRepository
    {
        private readonly ApplicationDbContext _Context;

        public ContentSubmissionRepo(ApplicationDbContext context)
        {
            _Context = context;
        }
        public async Task<IEnumerable<ContentSubmission>> GetAllAsync()
        {
            return await _Context.ContentSubmission.ToListAsync();
        }
        public async Task<ContentSubmission>GetByIdAsync(int id)
        {
            return await _Context.ContentSubmission.FindAsync(id);
        }
        public async Task<ContentSubmission> CreateAsync(ContentSubmission contentSubmission)
        {
            await _Context.ContentSubmission.AddAsync(contentSubmission);
            await _Context.SaveChangesAsync();
            return contentSubmission;
        }

        public async Task<bool>DeleteAsync(int id)
        {
            var contentSubmission = await _Context.ContentSubmission.FindAsync(id);
            if (contentSubmission == null)
            {
                return false;
            }
            _Context.ContentSubmission.Remove(contentSubmission);
            await _Context.SaveChangesAsync();
            return true;
        }


        public async Task<bool>UpdateAsync(ContentSubmission content)
        {
            var existingContent = await _Context.ContentSubmission.FindAsync(content.Id);
            if (existingContent == null)
            {
                return false;
            }
            existingContent.TextContent = content.TextContent;
            existingContent.ToxicityScore = content.ToxicityScore;
            existingContent.Status = content.Status;
            await _Context.SaveChangesAsync();
            return true;
        }
    
    }
}
