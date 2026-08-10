using Microsoft.EntityFrameworkCore;
using AutomatedContentGuard.Models;

namespace AutomatedContentGuard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { }

        public DbSet<ContentSubmission> ContentSubmissions { get; set; }
        public DbSet<ForbiddenWord> ForbiddenWords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Force exact table names in PostgreSQL to avoid pluralization mismatched errors
            modelBuilder.Entity<ContentSubmission>().ToTable("ContentSubmissions");
            modelBuilder.Entity<ForbiddenWord>().ToTable("ForbiddenWords");
        }
    }
}
