using Microsoft.EntityFrameworkCore;
using AutomatedContentGuard.Models;
namespace AutomatedContentGuard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { }

        public DbSet<ContentSubmission>ContentSubmission { get; set; }
        public DbSet<ForbiddenWord>ForbiddenWords { get; set; }
    }
}
