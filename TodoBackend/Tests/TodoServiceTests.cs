using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Data;
using TodoBackend.Services;
using TodoBackend.Models;

namespace TodoBackend.Tests
{
    public class TodoServiceTests
    {
        private readonly DbContextOptions<TodoDbContext> _options;
        private readonly TodoDbContext _context;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "TodoListTest")
                .Options;

            _context = new TodoDbContext(_options);
            _service = new TodoService(_context);

            // Clean database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateTodoAsync_ShouldCreateNewTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Test Todo",
                Priority = Priority.Medium
            };

            // Act
            var result = await _service.CreateTodoAsync(todo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Todo", result.Title);
            Assert.Equal(Priority.Medium, result.Priority);
            Assert.NotEqual(default, result.CreatedAt);
        }

        [Fact]
        public async Task GetAllTodosAsync_ShouldReturnAllTodos()
        {
            // Arrange
            await _service.CreateTodoAsync(new Todo { Title = "Todo 1" });
            await _service.CreateTodoAsync(new Todo { Title = "Todo 2" });

            // Act
            var todos = await _service.GetAllTodosAsync();

            // Assert
            Assert.Equal(2, todos.Count());
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldUpdateExistingTodo()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo { Title = "Original" });
            var updatedTodo = new Todo
            {
                Id = todo.Id,
                Title = "Updated",
                Priority = Priority.High
            };

            // Act
            var result = await _service.UpdateTodoAsync(todo.Id, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
            Assert.Equal(Priority.High, result.Priority);
            Assert.NotNull(result.UpdatedAt);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldDeleteTodo()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo { Title = "To Delete" });

            // Act
            var result = await _service.DeleteTodoAsync(todo.Id);

            // Assert
            Assert.True(result);
            var todos = await _service.GetAllTodosAsync();
            Assert.Empty(todos);
        }
    }
} 