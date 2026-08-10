using AutomatedContentGuard.DTOs;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutomatedContentGuard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForbiddenWordsController : ControllerBase
    {
        private readonly IForbiddenWordService _forbiddenWordService;

        public ForbiddenWordsController(IForbiddenWordService forbiddenWordService)
        {
            _forbiddenWordService = forbiddenWordService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForbiddenWord>>> GetAll()
        {
            
            var words = await _forbiddenWordService.GetAllAsync();
            return Ok(words);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForbiddenWord>> GetById(int id)
        {
            var word = await _forbiddenWordService.GetByIdAsync(id);

            if (word == null)
            {
                return NotFound(new { message = $"Forbidden word with ID {id} not found." });
            }

            return Ok(word);
        }

        [HttpPost]
        public async Task<ActionResult<ForbiddenWord>> Create([FromBody] CreateForbiddenWordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdWord = await _forbiddenWordService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdWord.Id }, createdWord);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
           
            var deleted = await _forbiddenWordService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = $"Forbidden word with ID {id} not found." });
            }

            return NoContent();
        }
    }
}