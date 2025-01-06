using Microsoft.AspNetCore.Mvc;
using TodoBackend.Models;
using TodoBackend.Services;
using System.Text.Json;

namespace TodoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITodoService todoService, ILogger<TodosController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            try
            {
                var todos = await _todoService.GetAllTodosAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todos");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todo)
        {
            try
            {
                if (todo == null)
                    return BadRequest(new { error = "Todo is null" });

                _logger.LogInformation("Received todo: {Todo}", JsonSerializer.Serialize(todo));

                // Set default values
                todo.CreatedAt = DateTime.UtcNow;
                todo.Completed = false;
                if (todo.Priority == 0) // If not set
                    todo.Priority = Priority.Low;

                var createdTodo = await _todoService.CreateTodoAsync(todo);
                return CreatedAtAction(nameof(GetTodos), new { id = createdTodo.Id }, createdTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo todo)
        {
            try
            {
                if (id != todo.Id)
                    return BadRequest(new { error = "Id mismatch" });

                var updatedTodo = await _todoService.UpdateTodoAsync(id, todo);
                if (updatedTodo == null)
                    return NotFound(new { error = "Todo not found" });

                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var result = await _todoService.DeleteTodoAsync(id);
                if (!result)
                    return NotFound(new { error = "Todo not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}/priority")]
        public async Task<ActionResult<Todo>> UpdatePriority(int id, [FromBody] PriorityUpdateDto priorityUpdate)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                    return NotFound(new { error = "Todo not found" });

                todo.Priority = priorityUpdate.Priority;
                var updatedTodo = await _todoService.UpdateTodoAsync(id, todo);
                
                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo priority");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class PriorityUpdateDto
        {
            public Priority Priority { get; set; }
        }
    }
} 