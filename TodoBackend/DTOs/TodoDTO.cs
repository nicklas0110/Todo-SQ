using TodoBackend.Models;

public record TodoDTO(
    int Id,
    string Title,
    Priority Priority,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? Deadline,
    bool IsCompleted
); 