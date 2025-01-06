using TodoBackend.DTOs;
using TodoBackend.Models;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace TodoBackend.DTOs;

public class CreateTodoDTO
{
    [Required]
    [MinLength(1)]
    [MaxLength(25)]
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public DateTime? Deadline { get; set; }
}

public record TitleUpdateDTO(string Title);
public record PriorityUpdateDTO(Priority Priority);
public record DeadlineUpdateDTO(DateTime? Deadline);