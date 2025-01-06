using Microsoft.AspNetCore.Mvc;
using TodoBackend.Models;
using TodoBackend.Services;
using TodoBackend.DTOs;

namespace TodoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            return todo == null ? NotFound() : Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
        {
            var createdTodo = await _todoService.CreateTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, createdTodo);
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
        public async Task<ActionResult<Todo>> UpdateTitle(int id, TitleUpdateDto dto)
        {
            var result = await _todoService.UpdateTodoTitleAsync(id, dto.Title);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}/priority")]
        public async Task<ActionResult<Todo>> UpdatePriority(int id, PriorityUpdateDto dto)
        {
            var result = await _todoService.UpdateTodoPriorityAsync(id, dto.Priority);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}/deadline")]
        public async Task<ActionResult<Todo>> UpdateDeadline(int id, DeadlineUpdateDto dto)
        {
            var result = await _todoService.UpdateTodoDeadlineAsync(id, dto.Deadline);
            return result == null ? NotFound() : Ok(result);
        }
    }
} 