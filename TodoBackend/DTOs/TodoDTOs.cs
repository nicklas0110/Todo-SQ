using TodoBackend.Models;

namespace TodoBackend.DTOs;

public record TitleUpdateDto(string Title);
public record PriorityUpdateDto(Priority Priority);
public record DeadlineUpdateDto(DateTime? Deadline); 