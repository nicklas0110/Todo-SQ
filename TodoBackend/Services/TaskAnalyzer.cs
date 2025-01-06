using System;
using System.Collections.Generic;
using System.Linq;
using TodoBackend.Models;

namespace TodoBackend.Services
{
    public class TaskAnalyzer : ITaskAnalyzer
    {
        public IEnumerable<Todo> GetOverdueTasks(IEnumerable<Todo> todos)
        {
            var now = DateTime.UtcNow;
            return todos.Where(t => 
                !t.Completed && 
                t.Deadline.HasValue && 
                t.Deadline.Value < now);
        }

        public Dictionary<Priority, int> GetPriorityDistribution(IEnumerable<Todo> todos)
        {
            var distribution = new Dictionary<Priority, int>();
            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
            {
                distribution[priority] = 0;
            }

            foreach (var todo in todos)
            {
                distribution[todo.Priority]++;
            }

            return distribution;
        }

        public double CalculateCompletionRate(IEnumerable<Todo> todos)
        {
            if (!todos.Any()) return 0;

            var totalTasks = todos.Count();
            var completedTasks = todos.Count(t => t.Completed);

            return (double)completedTasks / totalTasks * 100;
        }

        public IEnumerable<Todo> GetTasksRequiringImediateAttention(IEnumerable<Todo> todos)
        {
            var now = DateTime.UtcNow;
            var urgentDeadline = now.AddDays(2);

            return todos.Where(t => 
                !t.Completed &&
                ((t.Priority == Priority.High || t.Priority == Priority.Critical) ||
                (t.Deadline.HasValue && t.Deadline.Value <= urgentDeadline)))
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.Deadline);
        }
    }
} 