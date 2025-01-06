using TodoBackend.Models;

namespace TodoBackend.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetAllTodosAsync();
        Task<Todo?> GetTodoByIdAsync(int id);
        Task<Todo> CreateTodoAsync(Todo todo);
        Task<Todo?> UpdateTodoAsync(int id, Todo todo);
        Task<bool> DeleteTodoAsync(int id);
    }
} 