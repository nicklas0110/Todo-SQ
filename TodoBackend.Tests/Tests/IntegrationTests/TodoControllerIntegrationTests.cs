using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using TodoBackend.Models;
using TodoBackend.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoBackend.Data;
using TodoBackend.DTOs;
using TodoBackend;

namespace TodoBackend.Tests.IntegrationTests
{
    public class TodoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TodoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
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
                        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                    });
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GetTodos_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/todos");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedTodo()
        {
            // Arrange
            var newTodo = new Todo
            {
                Title = "Integration Test Todo",
                Priority = Priority.High
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(returnedTodo);
            Assert.Equal(newTodo.Title, returnedTodo.Title);
            Assert.Equal(newTodo.Priority, returnedTodo.Priority);
        }

        [Fact]
        public async Task UpdateTodo_ReturnsUpdatedTodo()
        {
            // Arrange
            var newTodo = new Todo { Title = "Original Title" };
            var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
            var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();

            // Act
            createdTodo!.Title = "Updated Title";
            var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{createdTodo.Id}", createdTodo);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            var updatedTodo = await updateResponse.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(updatedTodo);
            Assert.Equal("Updated Title", updatedTodo.Title);
        }

        [Fact]
        public async Task UpdateTodoTitle_ReturnsUpdatedTodo()
        {
            // Create a new todo first
            var newTodo = new Todo { Title = "Test Todo" };
            var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
            var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(createdTodo);

            // Update the title using PUT instead of PATCH
            var titleUpdate = new TitleUpdateDto("Updated Title");
            var response = await _client.PutAsJsonAsync($"/api/todos/{createdTodo.Id}/title", titleUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(updatedTodo);
            Assert.Equal("Updated Title", updatedTodo.Title);
        }

        [Fact]
        public async Task UpdateTodoPriority_ReturnsUpdatedTodo()
        {
            // Create a new todo first
            var newTodo = new Todo { Title = "Test Todo" };
            var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
            var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(createdTodo);

            // Update the priority using PUT instead of PATCH
            var priorityUpdate = new PriorityUpdateDto(Priority.High);
            var response = await _client.PutAsJsonAsync($"/api/todos/{createdTodo.Id}/priority", priorityUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(updatedTodo);
            Assert.Equal(Priority.High, updatedTodo.Priority);
        }

        [Fact]
        public async Task UpdateTodoDeadline_ReturnsUpdatedTodo()
        {
            // Create a new todo first
            var newTodo = new Todo { Title = "Test Todo" };
            var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
            var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(createdTodo);

            // Update the deadline using PUT instead of PATCH
            var deadline = DateTime.UtcNow.AddDays(1);
            var deadlineUpdate = new DeadlineUpdateDto(deadline);
            var response = await _client.PutAsJsonAsync($"/api/todos/{createdTodo.Id}/deadline", deadlineUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(updatedTodo);
            Assert.Equal(deadline.Date, updatedTodo.Deadline?.Date);
        }
    }
} 