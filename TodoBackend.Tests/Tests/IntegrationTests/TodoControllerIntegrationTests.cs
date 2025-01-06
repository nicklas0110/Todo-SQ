using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoBackend.Data;
using TodoBackend.DTOs;
using TodoBackend.Models;
using Xunit;

namespace TodoBackend.Tests.IntegrationTests
{
    public class TodoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "api/todos";
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IServiceScope _scope;
        private readonly TodoDbContext _context;

        public TodoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var dbName = $"TestingDb_{Guid.NewGuid()}";
            
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TodoDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });
                });
            });

            _client = _factory.CreateClient();
            
            // Create a scope to resolve the context
            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        }

        [Fact]
        public void Dispose()
        {
            _scope.Dispose();
        }

        private async Task ResetDatabaseAsync()
        {
            // Clear all todos
            if (_context.Todos.Any())
            {
                _context.Todos.RemoveRange(_context.Todos);
                await _context.SaveChangesAsync();
            }
        }

        // Tests that when the database is empty, the GET endpoint returns an empty list
        // This verifies the basic GET functionality and proper initialization state
        [Fact]
        public async Task GetTodos_WhenEmpty_ReturnsEmptyList()
        {
            // Arrange
            await ResetDatabaseAsync();

            // Act
            var response = await _client.GetAsync(_baseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var todos = await response.Content.ReadFromJsonAsync<IEnumerable<Todo>>();
            todos.Should().NotBeNull();
            todos.Should().BeEmpty();
        }

        // Tests that when todos exist in the database, the GET endpoint returns all of them
        // This verifies that the GET endpoint properly retrieves multiple items
        [Fact]
        public async Task GetTodos_WithItems_ReturnsAllTodos()
        {
            // Arrange
            await ResetDatabaseAsync();
            var todo1 = await CreateTestTodo();
            var todo2 = await CreateTestTodo();

            // Act
            var response = await _client.GetAsync(_baseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var todos = await response.Content.ReadFromJsonAsync<IEnumerable<Todo>>();
            todos.Should().NotBeNull();
            todos!.Should().HaveCount(2);
            todos.Should().Contain(t => t.Id == todo1.Id);
            todos.Should().Contain(t => t.Id == todo2.Id);
        }

        // Tests that getting a specific todo by ID returns the correct todo
        // This verifies the GET by ID endpoint works correctly for existing todos
        [Fact]
        public async Task GetTodoById_WithValidId_ReturnsTodo()
        {
            // Arrange
            var todo = await CreateTestTodo();

            // Act
            var response = await _client.GetAsync($"{_baseUrl}/{todo.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            returnedTodo.Should().NotBeNull();
            returnedTodo!.Id.Should().Be(todo.Id);
        }

        // Tests that requesting a non-existent todo returns a 404 Not Found
        // This verifies proper error handling for invalid IDs
        [Fact]
        public async Task GetTodoById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync($"{_baseUrl}/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Tests that creating a todo with an empty title fails validation
        // This verifies the required field validation for the title
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public async Task CreateTodo_WithEmptyTitle_ReturnsBadRequest(string emptyTitle)
        {
            // Arrange
            await ResetDatabaseAsync();
            var newTodo = new CreateTodoDTO { Title = emptyTitle };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrl, newTodo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Tests that creating a todo with valid title lengths succeeds
        // This verifies the title length validation works correctly
        [Theory]
        [InlineData("a")]
        [InlineData("Valid Title")]
        [InlineData("1234567890123456789012345")]
        public async Task CreateTodo_WithValidTitle_ReturnsCreated(string validTitle)
        {
            // Arrange
            await ResetDatabaseAsync();
            var newTodo = new CreateTodoDTO { Title = validTitle };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrl, newTodo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // Tests that creating a todo with invalid titles (empty or too long) fails
        // This verifies both minimum and maximum length validation rules
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("123456789012345678901234567890")]
        public async Task CreateTodo_WithInvalidTitle_ReturnsBadRequest(string invalidTitle)
        {
            // Arrange
            await ResetDatabaseAsync();
            var newTodo = new CreateTodoDTO { Title = invalidTitle };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrl, newTodo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Tests that updating a todo with valid data succeeds and returns updated values
        // This verifies the general update endpoint works correctly
        [Fact]
        public async Task UpdateTodo_WithValidData_ReturnsUpdatedTodo()
        {
            // Arrange
            var todo = await CreateTestTodo();
            todo.Title = "Updated Title";
            todo.Priority = Priority.High;

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrl}/{todo.Id}", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            updatedTodo.Should().NotBeNull();
            updatedTodo!.Title.Should().Be("Updated Title");
            updatedTodo.Priority.Should().Be(Priority.High);
            updatedTodo.UpdatedAt.Should().NotBeNull();
        }

        // Tests that updating a non-existent todo returns 404 Not Found
        // This verifies proper error handling for updates
        [Fact]
        public async Task UpdateTodo_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var todo = await CreateTestTodo();
            todo.Title = "Updated Title";

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrl}/99999", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Tests that deleting a todo removes it from the database
        // This verifies both the delete operation and subsequent GET returns 404
        [Fact]
        public async Task DeleteTodo_WithValidId_RemovesTodo()
        {
            // Arrange
            var todo = await CreateTestTodo();

            // Act
            var deleteResponse = await _client.DeleteAsync($"{_baseUrl}/{todo.Id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            // Verify todo is gone
            var getResponse = await _client.GetAsync($"{_baseUrl}/{todo.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Tests that attempting to delete a non-existent todo returns 404 Not Found
        // This verifies proper error handling for delete operations
        [Fact]
        public async Task DeleteTodo_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.DeleteAsync($"{_baseUrl}/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Tests that updating only the title field doesn't affect other fields
        // This verifies the partial update endpoint maintains data integrity
        [Fact]
        public async Task UpdateTodoTitle_WithValidData_UpdatesOnlyTitle()
        {
            // Arrange
            var todo = await CreateTestTodo();
            var originalPriority = todo.Priority;

            // Act
            var response = await _client.PutAsync(
                $"{_baseUrl}/{todo.Id}/title",
                JsonContent.Create(new TitleUpdateDTO("Updated Title"))
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            updatedTodo.Should().NotBeNull();
            updatedTodo!.Title.Should().Be("Updated Title");
            updatedTodo.Priority.Should().Be(originalPriority); // Verify other fields unchanged
        }

        // Tests that concurrent updates to the same todo are handled properly
        // This verifies the application's behavior under concurrent modifications
        [Fact]
        public async Task UpdateTodo_ConcurrentUpdates_HandlesCorrectly()
        {
            // Arrange
            var todo = await CreateTestTodo();
            
            // Act
            var task1 = _client.PutAsync(
                $"{_baseUrl}/{todo.Id}/title",
                JsonContent.Create(new TitleUpdateDTO("Update 1"))
            );
            var task2 = _client.PutAsync(
                $"{_baseUrl}/{todo.Id}/title",
                JsonContent.Create(new TitleUpdateDTO("Update 2"))
            );

            // Assert
            await Task.WhenAll(task1, task2);
            var finalTodo = await _client.GetAsync($"{_baseUrl}/{todo.Id}");
            var content = await finalTodo.Content.ReadFromJsonAsync<Todo>();
            content.Should().NotBeNull();
            content!.Title.Should().BeOneOf("Update 1", "Update 2");
        }

        private async Task<Todo> CreateTestTodo()
        {
            var newTodo = new CreateTodoDTO
            {
                Title = $"Test-{Guid.NewGuid().ToString().Substring(0, 5)}",
                Priority = Priority.Low
            };
            
            var createResponse = await _client.PostAsJsonAsync(_baseUrl, newTodo);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var todo = await createResponse.Content.ReadFromJsonAsync<Todo>();
            todo.Should().NotBeNull();
            return todo!;
        }
    }
} 