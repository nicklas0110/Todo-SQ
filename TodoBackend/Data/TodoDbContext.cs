using Microsoft.EntityFrameworkCore;
using TodoBackend.Models;

namespace TodoBackend.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>()
                .Property(t => t.IsCompleted)
                .HasDefaultValue(false);
        }
    }
} 