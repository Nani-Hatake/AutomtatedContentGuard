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

        // Unwraps the exact PostgreSQL database error (e.g., duplicate key, missing column, null constraint)
        string detailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

        return StatusCode(500, new { 
            message = "Database Save Error", 
            details = detailedError 
        });
    }
}
