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
            return await _context.Todos.ToListAsync();
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdateTodoAsync(int id, Todo todo)
        {
            if (id != todo.Id)
                return null;

            _context.Entry(todo).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return todo;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TodoExists(id))
                    return null;
                throw;
            }
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

        private async Task<bool> TodoExists(int id)
        {
            return await _context.Todos.AnyAsync(e => e.Id == id);
        }
    }
} 