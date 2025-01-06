using System.Text.Json.Serialization;

namespace TodoBackend.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Priority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public class Todo
    {
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        [JsonPropertyName("deadline")]
        public DateTime? Deadline { get; set; }
        
        [JsonPropertyName("priority")]
        public Priority Priority { get; set; }
        
        public bool IsCompleted { get; set; } = false;
    }
} 