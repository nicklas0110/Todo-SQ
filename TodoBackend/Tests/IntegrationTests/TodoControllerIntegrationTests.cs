using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using TodoBackend.Models;

namespace TodoBackend.Tests.IntegrationTests
{
    public class TodoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public TodoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
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
    }
} 