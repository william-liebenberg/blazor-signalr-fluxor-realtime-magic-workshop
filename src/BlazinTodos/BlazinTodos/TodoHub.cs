using BlazinTodos.Shared;

using Microsoft.AspNetCore.SignalR;

namespace BlazinTodos;

public class TodoHub : Hub
{
    public async Task BroadcastNewTodo(ToDoItem newToDo)
    {
        await Clients.All.SendAsync("ReceiveNewTodo", newToDo);
    }

    public async Task BroadcastUpdatedTodo(ToDoItem updatedToDo)
    {
        await Clients.All.SendAsync("ReceiveUpdatedTodo", updatedToDo);
    }

    public async Task BroadcastDeletedTodo(Guid id)
    {
        await Clients.All.SendAsync("ReceiveDeletedTodo", id);
    }

    public async Task BroadcastCompletedTodo(ToDoItem completedToDo)
    {
        await Clients.All.SendAsync("ReceiveCompletedTodo", completedToDo);
    }

    public async Task BroadcastResettedTodo(ToDoItem completedToDo)
    {
        await Clients.All.SendAsync("ReceiveResettedTodo", completedToDo);
    }
}