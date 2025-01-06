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

        /// <summary>
        /// Gets all overdue todos with complex filtering logic:
        /// - Checks if todo has a deadline
        /// - Checks if deadline is in the past
        /// - Filters based on completion status
        /// Uses explicit loop and conditions for better visibility of logic paths
        /// </summary>
        public async Task<List<Todo>> GetOverdueTodosAsync(bool includeCompleted = false)
        {
            var allTodos = await _repository.GetAllAsync();
            var now = DateTime.UtcNow;
            var overdueTodos = new List<Todo>();

            foreach (var todo in allTodos)  // Loop through all todos
            {
                if (todo.Deadline.HasValue)  // First condition: Has deadline?
                {
                    if (todo.Deadline.Value < now)  // Second condition: Is deadline in past?
                    {
                        if (!todo.IsCompleted)  // Third condition: Is not completed?
                        {
                            overdueTodos.Add(todo);  // Path 1: Not completed overdue todos
                        }
                        else if (includeCompleted)  // Fourth condition: Include completed?
                        {
                            overdueTodos.Add(todo);  // Path 2: Completed overdue todos
                        }
                    }
                    // Path 3: Future deadline (implicit skip)
                }
                // Path 4: No deadline (implicit skip)
            }

            return overdueTodos;
        }
    }
} 