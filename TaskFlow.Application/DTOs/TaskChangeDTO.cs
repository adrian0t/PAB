namespace TaskFlow.Application.DTOs;

public class TaskChangeDTO
{
    public string operation { get; set; } = null!;
    public string? value { get; set; }
}
