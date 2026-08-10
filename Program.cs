using AutomatedContentGuard.Data;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Repositories;
using AutomatedContentGuard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register DbContext with Neon PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Register Repositories
builder.Services.AddScoped<IContentRepository, ContentSubmissionRepo>();
builder.Services.AddScoped<IForbiddenWordRepository, ForbiddenWordRepo>();

// 5. Register Services
builder.Services.AddScoped<IContentSubmissionService, ContentSubmissionService>();
builder.Services.AddScoped<IForbiddenWordService, ForbiddenWordService>();

// Register HttpClient for Gemini/HuggingFace Moderation
builder.Services.AddHttpClient<GeminiModerationService>();

var app = builder.Build();

// CRITICAL FIX: Place CORS at the very top of the HTTP pipeline
// so it applies even if an unhandled exception or 500 error occurs
app.UseCors("AllowAll");

// AUTO-MIGRATE / CREATE DB TABLES ON STARTUP
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB Initialization Error: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
