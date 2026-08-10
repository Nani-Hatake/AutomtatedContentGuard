using AutomatedContentGuard.Data;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Repositories;
using AutomatedContentGuard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Add Controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register ApplicationDbContext with PostgreSQL (Neon DB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Register Repositories
builder.Services.AddScoped<IContentRepository, ContentSubmissionRepo>();
builder.Services.AddScoped<IForbiddenWordRepository, ForbiddenWordRepo>();

// 5. Register Services
builder.Services.AddScoped<IContentSubmissionService, ContentSubmissionService>();
builder.Services.AddScoped<IForbiddenWordService, ForbiddenWordService>();

// HttpClient Registration
builder.Services.AddHttpClient<GeminiModerationService>();

var app = builder.Build();

// AUTO-MIGRATE / CREATE NEON DB TABLES ON STARTUP
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// 6. Enable CORS Middleware (Must be placed early)
app.UseCors("AllowAll");

// 7. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
