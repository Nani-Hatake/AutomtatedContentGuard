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
            // Fallback default values in case AI service fails or times out
            double toxicityScore = 0.0;
            bool isFlagged = false;
            string status = "Approved";

            try
            {
                // 1. Analyze text with AI Service
                var aiResult = await _geminiService.AnalyzeTextAsync(dto.TextContent);
                if (aiResult != null)
                {
                    toxicityScore = aiResult.ToxicityScore;
                    isFlagged = aiResult.IsFlagged;
                    status = isFlagged ? "Flagged" : "Approved";
                }
            }
            catch (Exception ex)
            {
                // Log AI service error without crashing the database save
                Console.WriteLine($"[AI Moderation Fallback Triggered]: {ex.Message}");
            }

            // 2. Map properties with explicit UTC timestamps for Npgsql / PostgreSQL compatibility
            var submission = new ContentSubmission
            {
                TextContent = dto.TextContent,
                ToxicityScore = (int)Math.Round(toxicityScore),
                IsFlagged = isFlagged,
                Status = status,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                SubmittedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };

            // 3. Save to Neon PostgreSQL Database
            return await _contentRepo.CreateAsync(submission);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _contentRepo.DeleteAsync(id);
        }
    }
}
