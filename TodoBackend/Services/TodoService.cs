using Microsoft.EntityFrameworkCore;
using TodoBackend.Data;
using TodoBackend.Models;

namespace TodoBackend.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;

        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            return await _context.Todos
                .OrderByDescending(t => t.Priority)  // Highest priority first
                .ThenBy(t => t.Deadline)            // Then by deadline if priority is equal
                .ThenByDescending(t => t.CreatedAt) // Then by creation date (newest first) if both priority and deadline are equal
                .ToListAsync();
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdateTodoAsync(int id, Todo updatedTodo)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return null;

            todo.Title = updatedTodo.Title;
            todo.Priority = updatedTodo.Priority;
            todo.Deadline = updatedTodo.Deadline;
            todo.Completed = updatedTodo.Completed;
            todo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Todo?> UpdateTodoPriorityAsync(int id, Priority priority)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return null;

            todo.Priority = priority;
            todo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdateTodoDeadlineAsync(int id, DateTime? deadline)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return null;

            todo.Deadline = deadline;
            todo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdateTodoTitleAsync(int id, string title)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return null;

            todo.Title = title;
            todo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return todo;
        }

        private async Task<bool> TodoExists(int id)
        {
            return await _context.Todos.AnyAsync(e => e.Id == id);
        }
    }
} 