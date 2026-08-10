using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;
using Microsoft.EntityFrameworkCore;

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

            // 2. Map properties with exact types & UTC timestamps for Npgsql / PostgreSQL
            var submission = new ContentSubmission
            {
                TextContent = dto.TextContent,
                ToxicityScore = toxicityScore, // Fixed: Pass double directly without int cast
                IsFlagged = isFlagged,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                SubmittedAt = DateTime.UtcNow
            };

            // 3. Save to Neon PostgreSQL Database with robust EF Core exception unwrapping
            try
            {
                return await _contentRepo.CreateAsync(submission);
            }
            catch (DbUpdateException dbEx)
            {
                // Pulls the EXACT inner database constraint/type error from PostgreSQL
                string postgresMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                Console.WriteLine($"[PostgreSQL Database Save Error]: {postgresMessage}");
                throw new Exception($"Database Save Failed: {postgresMessage}");
            }
            catch (Exception ex)
            {
                string generalMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"[Submission Creation Error]: {generalMessage}");
                throw new Exception($"Submission Failed: {generalMessage}");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _contentRepo.DeleteAsync(id);
        }
    }
}
