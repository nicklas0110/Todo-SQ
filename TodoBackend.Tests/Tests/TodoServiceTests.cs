using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Data;
using TodoBackend.Services;
using TodoBackend.Models;
using TodoBackend.Repositories;

namespace TodoBackend.Tests
{
    public class TodoServiceTests : IDisposable
    {
        private readonly TodoDbContext _context;
        private readonly TodoService _service;
        private readonly TodoRepository _repository;

        public TodoServiceTests()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TodoDbContext(options);
            _repository = new TodoRepository(_context);
            _service = new TodoService(_repository);
        }

        // Tests that creating a new todo sets all required fields
        // This verifies the basic creation functionality and proper initialization
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

        // Tests that the service returns all todos in the database
        // This verifies the basic retrieval functionality
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

        // Tests that updating an existing todo changes all specified fields
        // This verifies the update functionality maintains data integrity
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

        // Tests that deleting a todo removes it from the database
        // This verifies both the delete operation and subsequent queries
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

        // Tests that todos are returned in priority order (Critical -> High -> Low)
        // This verifies the ordering functionality of the service
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

        // Tests that todos with equal priorities are ordered by deadline
        // This verifies the secondary sorting criteria works correctly
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

        // Tests that updating only the title doesn't affect other fields
        // This verifies the partial update functionality maintains data integrity
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

        // Tests that updating only the deadline doesn't affect other fields
        // This verifies the partial update functionality for deadline
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

        // Tests that updating only the priority doesn't affect other fields
        // This verifies the partial update functionality for priority
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

        // Tests that updating priority through dedicated method works correctly
        // This verifies the specific priority update endpoint
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

        // Tests that updating deadline through dedicated method works correctly
        // This verifies the specific deadline update endpoint
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

        // Tests that updating title through dedicated method works correctly
        // This verifies the specific title update endpoint
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