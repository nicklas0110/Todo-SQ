using Xunit;
using Moq;
using FluentAssertions;
using TodoBackend.Models;
using TodoBackend.Services;
using TodoBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TodoBackend.Tests.Tests
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoService _service;
        private readonly DateTime _now;

        public TodoServiceTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _service = new TodoService(_mockRepo.Object);
            _now = DateTime.UtcNow;
        }

        /// <summary>
        /// Tests the GetOverdueTodosAsync method with various todo states:
        /// 1. Todo with no deadline (should be excluded)
        /// 2. Todo with future deadline (should be excluded)
        /// 3. Todo with past deadline, not completed (should be included)
        /// 4. Todo with past deadline, completed (should be included only when includeCompleted is true)
        /// 
        /// This test verifies both the filtering of overdue items and the handling of the includeCompleted parameter.
        /// It uses complex logic combining deadline checks and completion status.
        /// </summary>
        [Fact]
        public async Task GetOverdueTodos_WithVariousConditions_ReturnsCorrectTodos()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo 
                { 
                    Id = 1, 
                    Title = "No deadline",
                    Deadline = null
                },
                new Todo 
                { 
                    Id = 2, 
                    Title = "Future deadline", 
                    Deadline = _now.AddDays(1)
                },
                new Todo 
                { 
                    Id = 3, 
                    Title = "Past deadline", 
                    Deadline = _now.AddDays(-1),
                    IsCompleted = false
                },
                new Todo 
                { 
                    Id = 4, 
                    Title = "Past deadline completed", 
                    Deadline = _now.AddDays(-1),
                    IsCompleted = true
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(todos);

            // Act
            var overdueTodos = await _service.GetOverdueTodosAsync(includeCompleted: false);
            var overdueTodosWithCompleted = await _service.GetOverdueTodosAsync(includeCompleted: true);

            // Assert
            overdueTodos.Should().HaveCount(1);
            overdueTodos.Should().Contain(t => t.Id == 3);

            overdueTodosWithCompleted.Should().HaveCount(2);
            overdueTodosWithCompleted.Should().Contain(t => t.Id == 3);
            overdueTodosWithCompleted.Should().Contain(t => t.Id == 4);
        }

        /// <summary>
        /// Tests the GetOverdueTodosAsync method with different completion states.
        /// Uses Theory with InlineData to test two scenarios:
        /// 1. includeCompleted = true: Should return both completed and uncompleted overdue todos (2 items)
        /// 2. includeCompleted = false: Should return only uncompleted overdue todos (1 item)
        /// 
        /// This test verifies the completion filtering logic using parameterized test cases.
        /// </summary>
        /// <param name="includeCompleted">Whether to include completed todos in the results</param>
        /// <param name="expectedCount">Expected number of todos in the result</param>
        [Theory]
        [InlineData(true, 2)]  // Should include completed items
        [InlineData(false, 1)] // Should exclude completed items
        public async Task GetOverdueTodos_WithDifferentCompletionStates_ReturnsCorrectCount(
            bool includeCompleted, int expectedCount)
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo 
                { 
                    Id = 1, 
                    Title = "Past deadline", 
                    Deadline = _now.AddDays(-1),
                    IsCompleted = false
                },
                new Todo 
                { 
                    Id = 2, 
                    Title = "Past deadline completed", 
                    Deadline = _now.AddDays(-1),
                    IsCompleted = true
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(todos);

            // Act
            var overdueTodos = await _service.GetOverdueTodosAsync(includeCompleted);

            // Assert
            overdueTodos.Should().HaveCount(expectedCount);
        }
    }
} 