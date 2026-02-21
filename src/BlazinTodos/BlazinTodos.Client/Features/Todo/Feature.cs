using Fluxor;

namespace BlazinTodos.Client.Features.Todo;

public class Feature : Feature<ToDoState>
{
    public override string GetName() => "ToDo";
    protected override ToDoState GetInitialState() => new();
}