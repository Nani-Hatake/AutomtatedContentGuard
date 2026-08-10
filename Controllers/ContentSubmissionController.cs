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
                // Logs the exact error in Render dashboard logs so you can inspect the stack trace
                Console.WriteLine($"[GetAll Submissions Error]: {ex.ToString()}");
                
                // Returns an empty array with 200 OK to keep CORS alive and prevent the frontend from hard crashing
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
                Console.WriteLine($"[GetById Error]: {ex.ToString()}");
                return StatusCode(500, new { message = "An error occurred retrieving the submission.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContentSubmission>> Create([FromBody] CreateContentSubmissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSubmission = await _contentSubmissionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdSubmission.Id }, createdSubmission);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Create Submission Error]: {ex.ToString()}");
                return StatusCode(500, new { message = "An error occurred processing the submission.", details = ex.Message });
            }
        }
    }
}
