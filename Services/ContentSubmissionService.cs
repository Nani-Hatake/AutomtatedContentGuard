using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;

namespace AutomatedContentGuard.Services
{
    public class ContentSubmissionService : IContentSubmissionService
    {
        private readonly IContentRepository _contentRepo;
        private readonly GeminiModerationService _geminiService;

        public ContentSubmissionService(
            IContentRepository contentRepo,
            GeminiModerationService geminiService)
        {
            _contentRepo = contentRepo;
            _geminiService = geminiService;
        }

        public async Task<IEnumerable<ContentSubmission>> GetAllAsync()
        {
            return await _contentRepo.GetAllAsync();
        }

        public async Task<ContentSubmission?> GetByIdAsync(int id)
        {
            return await _contentRepo.GetByIdAsync(id);
        }

        public async Task<ContentSubmission> CreateAsync(CreateContentSubmissionDto dto)
        {
            // 1. Analyze text with Gemini AI
            var aiResult = await _geminiService.AnalyzeTextAsync(dto.TextContent);

            // 2. Map properties including Status & Flags
            var submission = new ContentSubmission
            {
                TextContent = dto.TextContent,
                ToxicityScore = (int)Math.Round(aiResult.ToxicityScore),
                IsFlagged = aiResult.IsFlagged,
                Status = aiResult.IsFlagged ? "Flagged" : "Approved", // Set Status dynamically
                CreatedAt = DateTime.UtcNow,
                SubmittedAt = DateTime.UtcNow
            };

            // 3. Save to Database
            return await _contentRepo.CreateAsync(submission);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _contentRepo.DeleteAsync(id);
        }
    }
}