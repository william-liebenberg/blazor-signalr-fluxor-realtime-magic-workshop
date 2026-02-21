using BlazinTodos.Shared;

using Fluxor;

namespace BlazinTodos.Client.Features.Todo;

public static class ToDoReducers
{
    [ReducerMethod]
    public static ToDoState ReduceInitToDosAction(ToDoState state, InitToDosAction action) =>
        state with { ToDos = new List<ToDoItem>(action.NewToDos) };

    [ReducerMethod]
    public static ToDoState ReduceAddToDoAction(ToDoState state, AddToDoAction action) =>
        state with { ToDos = new List<ToDoItem>(state.ToDos) { action.NewToDo } };

    [ReducerMethod]
    public static ToDoState ReduceUpdateToDoAction(ToDoState state, UpdateToDoAction action) =>
        state with { ToDos = state.ToDos.Select(todo => todo.Id == action.UpdatedToDo.Id ? action.UpdatedToDo : todo).ToList() };

    [ReducerMethod]
    public static ToDoState ReduceDeleteToDoAction(ToDoState state, DeleteToDoAction action) =>
        state with { ToDos = state.ToDos.Where(todo => todo.Id != action.TodoId).ToList() };

    [ReducerMethod]
    public static ToDoState ReduceCompleteToDoAction(ToDoState state, CompleteToDoAction action) =>
        state with { ToDos = state.ToDos.Select(todo => todo.Id == action.CompletedToDo.Id ? action.CompletedToDo : todo).ToList() };

    [ReducerMethod]
    public static ToDoState ReduceResettedToDoAction(ToDoState state, ResettedToDoAction action)
    {
        ToDoState toDoState = new()
        {
            ToDos = state.ToDos
                .Select(todo => todo.Id == action.ResettedToDo.Id ? action.ResettedToDo : todo)
                .ToList()
        };

        return toDoState;
    }
}
