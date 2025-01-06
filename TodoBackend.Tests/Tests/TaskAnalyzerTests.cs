using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TodoBackend.Models;
using TodoBackend.Services;

namespace TodoBackend.Tests
{
    public class TaskAnalyzerTests
    {
        private readonly TaskAnalyzer _analyzer;
        private readonly DateTime _now;

        public TaskAnalyzerTests()
        {
            _analyzer = new TaskAnalyzer();
            _now = DateTime.UtcNow;
        }

        // Tests that only incomplete tasks with past deadlines are identified as overdue
        // This verifies that:
        // - Completed tasks are excluded even if deadline passed
        // - Future deadlines are not included
        // - Tasks without deadlines are not included
        [Fact]
        public void GetOverdueTasks_WithOverdueTasks_ReturnsOnlyOverdueTasks()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo { Id = 1, Deadline = _now.AddDays(-1), Completed = false },
                new Todo { Id = 2, Deadline = _now.AddDays(1), Completed = false },
                new Todo { Id = 3, Deadline = _now.AddDays(-2), Completed = true },
                new Todo { Id = 4, Deadline = null, Completed = false }
            };

            // Act
            var overdueTasks = _analyzer.GetOverdueTasks(todos).ToList();

            // Assert
            Assert.Single(overdueTasks);
            Assert.Equal(1, overdueTasks[0].Id);
        }

        // Tests the completion rate calculation with different scenarios:
        // - Empty list (0%)
        // - Half completed (50%)
        // - All completed (100%)
        // This verifies the percentage calculation works correctly for all cases
        [Theory]
        [InlineData(0, 0)]    // Empty list
        [InlineData(2, 50)]   // Half completed
        [InlineData(4, 100)]  // All completed
        public void CalculateCompletionRate_WithVariousScenarios_ReturnsExpectedRate(
            int completedCount, double expectedRate)
        {
            // Arrange
            var todos = new List<Todo>();
            for (int i = 0; i < completedCount; i++)
            {
                todos.Add(new Todo { Completed = true });
            }
            for (int i = 0; i < 4 - completedCount; i++)
            {
                todos.Add(new Todo { Completed = false });
            }

            // Act
            var rate = _analyzer.CalculateCompletionRate(todos);

            // Assert
            Assert.Equal(expectedRate, rate);
        }

        // Tests that tasks are correctly prioritized based on both priority level and deadline
        // This verifies that:
        // - Critical priority tasks come first regardless of deadline
        // - Tasks with same deadline are ordered by priority
        // - All urgent tasks are included in correct order
        [Fact]
        public void GetTasksRequiringImediateAttention_WithVariousTasks_ReturnsCorrectPrioritizedList()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo { Id = 1, Priority = Priority.Low, Deadline = _now.AddDays(1) },
                new Todo { Id = 2, Priority = Priority.Critical, Deadline = _now.AddDays(3) },
                new Todo { Id = 3, Priority = Priority.Medium, Deadline = _now.AddDays(1) },
            };

            // Act
            var urgentTasks = _analyzer.GetTasksRequiringImediateAttention(todos).ToList();

            // Assert
            Assert.Equal(3, urgentTasks.Count);
            Assert.Equal(2, urgentTasks[0].Id); // Critical priority should be first
            Assert.Equal(3, urgentTasks[1].Id); // Medium priority with urgent deadline
            Assert.Equal(1, urgentTasks[2].Id); // Low priority with urgent deadline
        }

        // Tests that the count of tasks for each priority level is correctly calculated
        // This verifies that:
        // - Tasks are properly grouped by priority
        // - The count for each priority level is accurate
        // - All priority levels are represented in the distribution
        [Fact]
        public void GetPriorityDistribution_WithVariousPriorities_ReturnsCorrectDistribution()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo { Priority = Priority.Low },
                new Todo { Priority = Priority.Low },
                new Todo { Priority = Priority.Medium },
                new Todo { Priority = Priority.High },
                new Todo { Priority = Priority.Critical }
            };

            // Act
            var distribution = _analyzer.GetPriorityDistribution(todos);

            // Assert
            Assert.Equal(2, distribution[Priority.Low]);
            Assert.Equal(1, distribution[Priority.Medium]);
            Assert.Equal(1, distribution[Priority.High]);
            Assert.Equal(1, distribution[Priority.Critical]);
        }
    }
} 