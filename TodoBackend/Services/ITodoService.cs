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
        Task<Todo?> UpdateTodoTitleAsync(int id, string title);
        Task<Todo?> UpdateTodoPriorityAsync(int id, Priority priority);
        Task<Todo?> UpdateTodoDeadlineAsync(int id, DateTime? deadline);
    }
} 