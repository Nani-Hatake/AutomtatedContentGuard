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
            var submissions = await _contentSubmissionService.GetAllAsync();
            return Ok(submissions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentSubmission>> GetById(int id)
        {
            var submission = await _contentSubmissionService.GetByIdAsync(id);

            if (submission == null)
            {
                return NotFound(new { message = $"Submission with ID {id} not found." });
            }

            return Ok(submission);
        }

        [HttpPost]
        public async Task<ActionResult<ContentSubmission>> Create([FromBody] CreateContentSubmissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdSubmission = await _contentSubmissionService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdSubmission.Id }, createdSubmission);
        }
    }
}