using TodoBackend.Models;
using TodoBackend.Repositories;

namespace TodoBackend.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            var todos = await _repository.GetAllAsync();
            return todos
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.Deadline)
                .ThenByDescending(t => t.CreatedAt);
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            return await _repository.CreateAsync(todo);
        }

        public async Task<Todo?> UpdateTodoAsync(int id, Todo updatedTodo)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null) return null;

            todo.Title = updatedTodo.Title;
            todo.Priority = updatedTodo.Priority;
            todo.Deadline = updatedTodo.Deadline;
            todo.Completed = updatedTodo.Completed;
            todo.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(todo);
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<Todo?> UpdateTodoTitleAsync(int id, string title)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null) return null;

            todo.Title = title;
            todo.UpdatedAt = DateTime.UtcNow;
            return await _repository.UpdateAsync(todo);
        }

        public async Task<Todo?> UpdateTodoPriorityAsync(int id, Priority priority)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null) return null;

            todo.Priority = priority;
            todo.UpdatedAt = DateTime.UtcNow;
            return await _repository.UpdateAsync(todo);
        }

        public async Task<Todo?> UpdateTodoDeadlineAsync(int id, DateTime? deadline)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null) return null;

            todo.Deadline = deadline;
            todo.UpdatedAt = DateTime.UtcNow;
            return await _repository.UpdateAsync(todo);
        }
    }
} 