using AutomatedContentGuard.Data;
using AutomatedContentGuard.Interfaces;
using AutomatedContentGuard.Repositories;
using AutomatedContentGuard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers and API documentation services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Register ApplicationDbContext with SQL Server Connection String
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Repositories for Dependency Injection
builder.Services.AddScoped<IContentRepository, ContentSubmissionRepo>();
builder.Services.AddScoped<IForbiddenWordRepository, ForbiddenWordRepo>();

// 4. Register Services for Dependency Injection
builder.Services.AddScoped<IContentSubmissionService, ContentSubmissionService>();
builder.Services.AddScoped<IForbiddenWordService, ForbiddenWordService>();

// Registered via AddHttpClient to automatically inject HttpClient & IConfiguration
builder.Services.AddHttpClient<GeminiModerationService>();

var app = builder.Build();

// 5. Configure the HTTP request pipeline (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();