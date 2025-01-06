using TodoBackend.Models;
using System.Collections.Generic;

namespace TodoBackend.Services
{
    public interface ITaskAnalyzer
    {
        IEnumerable<Todo> GetOverdueTasks(IEnumerable<Todo> todos);
        Dictionary<Priority, int> GetPriorityDistribution(IEnumerable<Todo> todos);
        double CalculateCompletionRate(IEnumerable<Todo> todos);
        IEnumerable<Todo> GetTasksRequiringImediateAttention(IEnumerable<Todo> todos);
    }
} 