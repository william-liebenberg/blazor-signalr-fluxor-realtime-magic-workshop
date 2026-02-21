using BlazinTodos.Shared;

namespace BlazinTodos.Client.Features.Todo;

public record ToDoState
{
    public List<ToDoItem> ToDos { get; init; } = [];
}
