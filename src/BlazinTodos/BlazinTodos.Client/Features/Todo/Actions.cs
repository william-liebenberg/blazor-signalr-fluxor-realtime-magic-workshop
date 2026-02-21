using BlazinTodos.Shared;

namespace BlazinTodos.Client.Features.Todo;

public record InitToDosAction(ToDoItem[] NewToDos);
public record AddToDoAction(ToDoItem NewToDo);
public record UpdateToDoAction(ToDoItem UpdatedToDo);
public record DeleteToDoAction(Guid TodoId);
public record CompleteToDoAction(ToDoItem CompletedToDo);
public record ResettedToDoAction(ToDoItem ResettedToDo);

