using BlazinTodos.Shared;

namespace BlazinTodos;

public class ToDoRepository
{
    private readonly List<ToDoItem> _todos = [];

    public ToDoRepository()
    {
        _todos.Add(new ToDoItem { Id = Guid.NewGuid(), Title = "Learn Blazor", Completed = false });
        _todos.Add(new ToDoItem { Id = Guid.NewGuid(), Title = "Learn Fluxor", Completed = false });
        _todos.Add(new ToDoItem { Id = Guid.NewGuid(), Title = "Learn SignalR", Completed = false });
    }

    public IEnumerable<ToDoItem> GetAllTodos() => _todos;

    public Task AddTodoAsync(ToDoItem newToDo)
    {
        newToDo.Id = Guid.NewGuid();

        _todos.Add(newToDo);
        return Task.CompletedTask;
    }

    public Task<ToDoItem?> UpdateTodoAsync(Guid id, ToDoItem updatedToDo)
    {
        ToDoItem? todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            return Task.FromResult<ToDoItem?>(null);
        }

        todo.Title = updatedToDo.Title;
        todo.Completed = updatedToDo.Completed;
        return Task.FromResult<ToDoItem?>(todo);
    }

    public Task<bool> DeleteTodoAsync(Guid id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null) return Task.FromResult(false);
        _todos.Remove(todo);

        return Task.FromResult(true);
    }

    public Task<ToDoItem?> GetTodoAsync(Guid id)
    {
        return Task.FromResult(_todos.FirstOrDefault(t => t.Id == id));
    }
}
