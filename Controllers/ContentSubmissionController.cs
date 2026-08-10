using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutomatedContentGuard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentSubmissionsController : ControllerBase
    {
        private readonly IContentSubmissionService _contentSubmissionService;

        public ContentSubmissionsController(IContentSubmissionService contentSubmissionService)
        {
            _contentSubmissionService = contentSubmissionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentSubmission>>> GetAll()
        {
            try
            {
                var submissions = await _contentSubmissionService.GetAllAsync();
                return Ok(submissions ?? new List<ContentSubmission>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetAll Submissions Error]: {ex.Message}");
                return Ok(new List<ContentSubmission>());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentSubmission>> GetById(int id)
        {
            try
            {
                var submission = await _contentSubmissionService.GetByIdAsync(id);

                if (submission == null)
                {
                    return NotFound(new { message = $"Submission with ID {id} not found." });
                }

                return Ok(submission);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetById Error]: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred retrieving the submission.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContentSubmissionDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TextContent))
            {
                return BadRequest(new { message = "TextContent cannot be empty." });
            }

            try
            {
                var createdSubmission = await _contentSubmissionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdSubmission.Id }, createdSubmission);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[POST ContentSubmissions ERROR]: {ex}");

                string detailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return StatusCode(500, new { 
                    message = "Database Save Error", 
                    details = detailedError 
                });
            }
        }
    }
}
