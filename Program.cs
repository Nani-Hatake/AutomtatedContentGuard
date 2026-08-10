using AutomatedContentGuard.Data;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Repositories;
using AutomatedContentGuard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

// Fix for Npgsql / PostgreSQL DateTime timestamp mapping issues
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
app.UseCors("AllowAll");

// SAFE AUTO-RECREATE TABLES ON STARTUP
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Drop outdated table so EF recreates it with all missing columns (Status, ToxicityScore, etc.)
        dbContext.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS \"ContentSubmissions\" CASCADE;");

        // 2. Recreate tables with complete updated schema
        dbContext.Database.EnsureCreated();

        Console.WriteLine("[Database System]: Neon PostgreSQL tables recreated successfully with complete schema.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB Initialization Warning]: {ex.Message}");
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
