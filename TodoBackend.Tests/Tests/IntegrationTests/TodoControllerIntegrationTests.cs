using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
        private readonly string _baseUrl = "api/todos";
        private readonly WebApplicationFactory<Program> _factory;

        public TodoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
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
                        options.UseInMemoryDatabase("TestingDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        private async Task<Todo> CreateTestTodo()
        {
            var newTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Priority = Priority.Low
            };
            
            var createResponse = await _client.PostAsJsonAsync(_baseUrl, newTodo);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var todo = await createResponse.Content.ReadFromJsonAsync<Todo>();
            todo.Should().NotBeNull();
            return todo!;
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
        public async Task UpdateTodoTitle_ShouldUpdateTitle()
        {
            // Arrange
            var todo = await CreateTestTodo();

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
        }

        [Fact]
        public async Task UpdateTodoPriority_ShouldUpdatePriority()
        {
            // Arrange
            var todo = await CreateTestTodo();

            // Act
            var response = await _client.PutAsync(
                $"{_baseUrl}/{todo.Id}/priority",
                JsonContent.Create(new PriorityUpdateDTO(Priority.High))
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            updatedTodo.Should().NotBeNull();
            updatedTodo!.Priority.Should().Be(Priority.High);
        }

        [Fact]
        public async Task UpdateTodoDeadline_ShouldUpdateDeadline()
        {
            // Arrange
            var todo = await CreateTestTodo();
            var newDeadline = DateTime.UtcNow.Date.AddDays(1);

            // Act
            var response = await _client.PutAsync(
                $"{_baseUrl}/{todo.Id}/deadline",
                JsonContent.Create(new DeadlineUpdateDTO(newDeadline))
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
            updatedTodo.Should().NotBeNull();
            updatedTodo!.Deadline.Should().Be(newDeadline);
        }
    }
} 