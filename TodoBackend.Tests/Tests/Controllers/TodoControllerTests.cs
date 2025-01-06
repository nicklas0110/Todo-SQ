using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using TodoBackend.Controllers;
using TodoBackend.DTOs;
using TodoBackend.Models;
using TodoBackend.Services;

public class TodoControllerTests
{
    private readonly TodosController _controller;
    private readonly Mock<ITodoService> _mockTodoService;

    public TodoControllerTests()
    {
        _mockTodoService = new Mock<ITodoService>();
        _controller = new TodosController(_mockTodoService.Object);
    }

    /// <summary>
    /// Tests that updating a todo's priority with valid data succeeds.
    /// Verifies that the controller:
    /// 1. Correctly passes the priority to the service
    /// 2. Returns OK with the updated todo
    /// 3. Maintains data integrity
    /// </summary>
    [Fact]
    public async Task UpdateTodoPriority_WithValidData_UpdatesPriority()
    {
        // Arrange
        var todo = new Todo { Id = 1, Title = "Test", Priority = Priority.Low };
        var updatedTodo = new Todo { Id = 1, Title = "Test", Priority = Priority.High };
        var dto = new PriorityUpdateDTO(Priority.High);
        _mockTodoService.Setup(s => s.UpdateTodoPriorityAsync(1, Priority.High))
            .ReturnsAsync(updatedTodo);

        // Act
        var result = await _controller.UpdateTodoPriority(1, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Todo>(okResult.Value);
        Assert.Equal(Priority.High, returnValue.Priority);
    }

    /// <summary>
    /// Tests input validation for priority updates.
    /// Verifies that the controller properly handles null DTOs
    /// by returning a BadRequest result with an error message.
    /// This is important for API robustness.
    /// </summary>
    [Theory]
    [InlineData(null)]
    public async Task UpdateTodoPriority_WithNullDTO_ReturnsBadRequest(PriorityUpdateDTO? dto)
    {
        // Act
        var result = await _controller.UpdateTodoPriority(1, dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Priority update data is required", badRequestResult.Value);
    }

    /// <summary>
    /// Tests error handling for non-existent todos.
    /// Verifies that the controller returns NotFound when
    /// attempting to update a todo that doesn't exist.
    /// This ensures proper API error responses.
    /// </summary>
    [Fact]
    public async Task UpdateTodoPriority_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var dto = new PriorityUpdateDTO(Priority.High);
        _mockTodoService.Setup(s => s.UpdateTodoPriorityAsync(It.IsAny<int>(), It.IsAny<Priority>()))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await _controller.UpdateTodoPriority(999, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests successful deadline updates.
    /// Verifies that the controller:
    /// 1. Accepts valid future dates
    /// 2. Returns OK with updated todo
    /// 3. Correctly updates the deadline
    /// </summary>
    [Fact]
    public async Task UpdateTodoDeadline_WithValidData_UpdatesDeadline()
    {
        // Arrange
        var deadline = DateTime.Now.AddDays(1);
        var todo = new Todo { Id = 1, Title = "Test" };
        var updatedTodo = new Todo { Id = 1, Title = "Test", Deadline = deadline };
        var dto = new DeadlineUpdateDTO(deadline);
        _mockTodoService.Setup(s => s.UpdateTodoDeadlineAsync(1, deadline))
            .ReturnsAsync(updatedTodo);

        // Act
        var result = await _controller.UpdateTodoDeadline(1, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Todo>(okResult.Value);
        Assert.Equal(deadline, returnValue.Deadline);
    }

    /// <summary>
    /// Tests input validation for deadline updates.
    /// Verifies that the controller properly handles null DTOs
    /// by returning a BadRequest result with an error message.
    /// This ensures API stability.
    /// </summary>
    [Theory]
    [InlineData(null)]
    public async Task UpdateTodoDeadline_WithNullDTO_ReturnsBadRequest(DeadlineUpdateDTO? dto)
    {
        // Act
        var result = await _controller.UpdateTodoDeadline(1, dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Deadline update data is required", badRequestResult.Value);
    }

    /// <summary>
    /// Tests business rule validation for deadlines.
    /// Verifies that attempting to set a past deadline
    /// results in an ArgumentException. This ensures
    /// todos can only be scheduled for the future.
    /// </summary>
    [Fact]
    public async Task UpdateTodoDeadline_WithPastDate_HandlesPastDateValidation()
    {
        // Arrange
        var pastDate = DateTime.Now.AddDays(-1);
        var dto = new DeadlineUpdateDTO(pastDate);

        // Mock service to throw ArgumentException for past date
        _mockTodoService.Setup(s => s.UpdateTodoDeadlineAsync(1, pastDate))
            .ThrowsAsync(new ArgumentException("Deadline cannot be in the past"));

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _controller.UpdateTodoDeadline(1, dto));
    }

    /// <summary>
    /// Tests error handling for non-existent todos in deadline updates.
    /// Verifies that the controller returns NotFound when
    /// attempting to update a deadline for a non-existent todo.
    /// This ensures consistent 404 responses across the API.
    /// </summary>
    [Fact]
    public async Task UpdateTodoDeadline_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var deadline = DateTime.Now.AddDays(1);
        var dto = new DeadlineUpdateDTO(deadline);
        _mockTodoService.Setup(s => s.UpdateTodoDeadlineAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await _controller.UpdateTodoDeadline(999, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
} 