using BlazinTodos.Client.Features.Todo;
using BlazinTodos.Shared;

using Microsoft.AspNetCore.SignalR.Client;

namespace BlazinTodos.Client;

public class TodoHubActionBinder
{
    private readonly HubConnection _hubConnection;
    private readonly Fluxor.IDispatcher _dispatcher;

    public TodoHubActionBinder(HubConnection hubConnection, Fluxor.IDispatcher dispatcher)
    {
        _hubConnection = hubConnection;
        _dispatcher = dispatcher;
    }

    public async Task Bind()
    {
        _hubConnection.On<ToDoItem>("ReceiveNewTodo", newItem =>
            _dispatcher.Dispatch(new AddToDoAction(newItem)));

        _hubConnection.On<ToDoItem>("ReceiveUpdatedTodo", updatedItem =>
            _dispatcher.Dispatch(new UpdateToDoAction(updatedItem)));

        _hubConnection.On<Guid>("ReceiveDeletedTodo", todoId =>
            _dispatcher.Dispatch(new DeleteToDoAction(todoId)));

        _hubConnection.On<ToDoItem>("ReceiveCompletedTodo", completedItem =>
            _dispatcher.Dispatch(new CompleteToDoAction(completedItem)));

        _hubConnection.On<ToDoItem>("ReceiveResettedTodo", resettedItem =>
            _dispatcher.Dispatch(new ResettedToDoAction(resettedItem)));

        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }
}
