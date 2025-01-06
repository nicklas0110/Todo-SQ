using TodoBackend.DTOs;
using TodoBackend.Models;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace TodoBackend.DTOs;

public class CreateTodoDTO
{
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public DateTime? Deadline { get; set; }
}

public record TitleUpdateDTO(string Title);
public record PriorityUpdateDTO(Priority Priority);
public record DeadlineUpdateDTO(DateTime? Deadline);