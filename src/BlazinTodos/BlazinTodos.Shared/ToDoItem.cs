namespace BlazinTodos.Shared;

public class ToDoItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Completed { get; set; } = false;
}