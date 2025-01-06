using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Data;
using TodoBackend.Services;
using TodoBackend.Models;

namespace TodoBackend.Tests
{
    public class TodoServiceTests : IDisposable
    {
        private readonly TodoDbContext _context;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TodoDbContext(options);
            _service = new TodoService(_context);
        }

        [Fact]
        public async Task CreateTodoAsync_ShouldCreateNewTodo()
        {
            // Arrange
            var todo = new Todo { Title = "Test Todo", Priority = Priority.Medium };

            // Act
            var result = await _service.CreateTodoAsync(todo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Todo", result.Title);
            Assert.Equal(Priority.Medium, result.Priority);
            Assert.NotEqual(default(DateTime), result.CreatedAt);
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
            var todo = await _service.CreateTodoAsync(new Todo { Title = "Original", Priority = Priority.Low });
            Assert.NotNull(todo);

            var updatedTodo = new Todo
            {
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
        public async Task DeleteTodoAsync_ShouldDeleteExistingTodo()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo { Title = "To Delete" });
            Assert.NotNull(todo);

            // Act
            var result = await _service.DeleteTodoAsync(todo.Id);

            // Assert
            Assert.True(result);
            var todos = await _service.GetAllTodosAsync();
            Assert.Empty(todos);
        }

        [Fact]
        public async Task GetAllTodosAsync_ShouldReturnTodosInPriorityOrder()
        {
            // Arrange
            await _service.CreateTodoAsync(new Todo { Title = "Low Priority", Priority = Priority.Low });
            await _service.CreateTodoAsync(new Todo { Title = "High Priority", Priority = Priority.High });
            await _service.CreateTodoAsync(new Todo { Title = "Critical Priority", Priority = Priority.Critical });

            // Act
            var todos = (await _service.GetAllTodosAsync()).ToList();

            // Assert
            Assert.Equal(3, todos.Count);
            Assert.Equal(Priority.Critical, todos[0].Priority);
            Assert.Equal(Priority.High, todos[1].Priority);
            Assert.Equal(Priority.Low, todos[2].Priority);
        }

        [Fact]
        public async Task GetAllTodosAsync_ShouldOrderByDeadlineWhenPrioritiesEqual()
        {
            // Arrange
            var now = DateTime.UtcNow;
            await _service.CreateTodoAsync(new Todo 
            { 
                Title = "Later Deadline", 
                Priority = Priority.High,
                Deadline = now.AddDays(2)
            });
            await _service.CreateTodoAsync(new Todo 
            { 
                Title = "Earlier Deadline", 
                Priority = Priority.High,
                Deadline = now.AddDays(1)
            });

            // Act
            var todos = (await _service.GetAllTodosAsync()).ToList();

            // Assert
            Assert.Equal(2, todos.Count);
            Assert.Equal("Earlier Deadline", todos[0].Title);
            Assert.Equal("Later Deadline", todos[1].Title);
        }

        [Fact]
        public async Task UpdateTodoTitle_ShouldUpdateTitleOnly()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo 
            { 
                Title = "Original Title",
                Priority = Priority.High,
                Completed = true
            });

            var updatedTodo = new Todo
            {
                Title = "New Title",
                Priority = todo.Priority,
                Completed = todo.Completed
            };

            // Act
            var result = await _service.UpdateTodoAsync(todo.Id, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal(todo.Priority, result.Priority);
            Assert.Equal(todo.Completed, result.Completed);
        }

        [Fact]
        public async Task UpdateTodoDeadline_ShouldUpdateDeadlineOnly()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo 
            { 
                Title = "Test Todo",
                Deadline = null
            });

            var newDeadline = DateTime.UtcNow.AddDays(1);
            var updatedTodo = new Todo
            {
                Title = todo.Title,
                Deadline = newDeadline
            };

            // Act
            var result = await _service.UpdateTodoAsync(todo.Id, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todo.Title, result.Title);
            Assert.Equal(newDeadline.Date, result.Deadline?.Date);
        }

        [Fact]
        public async Task UpdateTodoPriority_ShouldUpdatePriorityOnly()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo 
            { 
                Title = "Test Todo",
                Priority = Priority.Low
            });

            var updatedTodo = new Todo
            {
                Title = todo.Title,
                Priority = Priority.Critical
            };

            // Act
            var result = await _service.UpdateTodoAsync(todo.Id, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todo.Title, result.Title);
            Assert.Equal(Priority.Critical, result.Priority);
        }

        [Fact]
        public async Task UpdateTodoPriorityAsync_ShouldUpdatePriority()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo { Title = "Test", Priority = Priority.Low });
            Assert.NotNull(todo);

            // Act
            var result = await _service.UpdateTodoPriorityAsync(todo.Id, Priority.High);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Priority.High, result.Priority);
        }

        [Fact]
        public async Task UpdateTodoDeadlineAsync_ShouldUpdateDeadline()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo { Title = "Test" });
            Assert.NotNull(todo);
            var newDeadline = DateTime.UtcNow.AddDays(1);

            // Act
            var result = await _service.UpdateTodoDeadlineAsync(todo.Id, newDeadline);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newDeadline.Date, result.Deadline?.Date);
        }

        [Fact]
        public async Task UpdateTodoTitleAsync_ShouldUpdateTitle()
        {
            // Arrange
            var todo = await _service.CreateTodoAsync(new Todo { Title = "Original" });
            Assert.NotNull(todo);

            // Act
            var result = await _service.UpdateTodoTitleAsync(todo.Id, "Updated Title");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 