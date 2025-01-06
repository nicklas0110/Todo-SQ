using Microsoft.AspNetCore.Mvc;
using TodoBackend.Models;
using TodoBackend.Services;
using TodoBackend.DTOs;

namespace TodoBackend.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDTO dto)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                Priority = dto.Priority,
                Deadline = dto.Deadline
            };

            var created = await _todoService.CreateTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodo), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodo(int id, Todo todo)
        {
            var result = await _todoService.UpdateTodoAsync(id, todo);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodo(int id)
        {
            var result = await _todoService.DeleteTodoAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{id}/title")]
        public async Task<IActionResult> UpdateTodoTitle(int id, [FromBody] TitleUpdateDTO dto)
        {
            var todo = await _todoService.UpdateTodoTitleAsync(id, dto.Title);
            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpPut("{id}/priority")]
        public async Task<IActionResult> UpdateTodoPriority(int id, [FromBody] PriorityUpdateDTO dto)
        {
            var todo = await _todoService.UpdateTodoPriorityAsync(id, dto.Priority);
            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpPut("{id}/deadline")]
        public async Task<IActionResult> UpdateTodoDeadline(int id, [FromBody] DeadlineUpdateDTO dto)
        {
            var todo = await _todoService.UpdateTodoDeadlineAsync(id, dto.Deadline);
            if (todo == null)
                return NotFound();
            return Ok(todo);
        }
    }
} 