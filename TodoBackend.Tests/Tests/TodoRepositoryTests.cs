using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Data;
using TodoBackend.Models;
using TodoBackend.Repositories;

public class TodoRepositoryTests : IDisposable
{
    private readonly TodoDbContext _context;
    private readonly TodoRepository _repository;

    public TodoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TodoDbContext(options);
        _repository = new TodoRepository(_context);
    }

    [Fact]
    public async Task TodoExists_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var todo = new Todo { Title = "Test Todo" };
        var createdTodo = await _repository.CreateAsync(todo);
        Assert.NotNull(createdTodo);

        // Act
        var exists = await _repository.TodoExists(createdTodo.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task TodoExists_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        var nonExistingId = 99999;

        // Act
        var exists = await _repository.TodoExists(nonExistingId);

        // Assert
        Assert.False(exists);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 