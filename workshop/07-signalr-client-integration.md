# Section 7: SignalR Client Integration

> ⏱️ Estimated time: **15 minutes**

> **Recap:** In the previous section, you created the Fluxor building blocks inside `Features/ToDo/` — the `ToDoState` record, the `Feature` class, action records for each operation (init, add, update, delete, complete, reset), and reducer methods that produce new state in response to each action.
>
> ⬅️ Previous: [Section 6: Fluxor State, Actions & Reducers](06-fluxor-state-actions-reducers.md)

## Overview

We've built the backend (SignalR Hub + API endpoints) and the frontend state management (Fluxor). Now we need to connect them — when the server broadcasts a change via SignalR, we need to **dispatch the corresponding Fluxor action** so the UI updates automatically.

In this section we'll:

1. Install the SignalR client package
2. Register a `HubConnection` in the DI container
3. Create a `TodoHubActionBinder` that wires SignalR events to Fluxor actions

## Step 1: Install the SignalR client package

In the `BlazinTodos.Client` project, add the SignalR client package:

```ps1
dotnet add package Microsoft.AspNetCore.SignalR.Client
```

## Step 2: Register the HubConnection

Open `Program.cs` in the `BlazinTodos.Client` project and add the following using statement:

```cs
using Microsoft.AspNetCore.SignalR.Client;
```

Then register the `HubConnection` as a singleton service. The connection URL points to the `/todoHub` endpoint we mapped in our backend `Program.cs`:

```cs
builder.Services.AddSingleton<HubConnection>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/todoHub"))
        .WithAutomaticReconnect()
        .Build();
});

```

Key points:
- **`NavigationManager.ToAbsoluteUri`** resolves the relative hub URL against the app's base address
- **`WithAutomaticReconnect()`** ensures the connection is re-established if it drops
- We register it as a **singleton** so all components share the same connection

## Step 3: Create the TodoHubActionBinder

This is the glue between SignalR and Fluxor. Create a new file called `TodoHubActionBinder.cs` in the `BlazinTodos.Client` project. This class registers handlers for each SignalR event and dispatches the corresponding Fluxor action:

```csharp
using BlazinTodos.Client.Features.Todo;
using BlazinTodos.Shared;

using Microsoft.AspNetCore.SignalR.Client;

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

		// Connection check, start the SignalR hub connection if it is not already connected
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }
}
```

Here's what's happening:

- Each `_hubConnection.On<T>(...)` call registers a handler for a specific SignalR event (matching the event names in our `TodoHub` on the server-side application)
- When a message arrives, the handler **dispatches** a **Fluxor Action**
	- the reducer then updates the state
	- and any subscribing components re-render automatically
- The `Bind()` method also starts the SignalR connection if it isn't already connected

## Step 4: Register the TodoHubActionBinder

Back in `Program.cs`, register the `TodoHubActionBinder` so it can be injected into components:

```cs
builder.Services.AddScoped<TodoHubActionBinder>();
```

We'll call its `Bind()` method from our TodoList component in the next section, which means the SignalR connection is established when the user navigates to the ToDo page.

## Step 5: Commit your changes

```powershell
git add .
git commit -m "Add SignalR client integration with Fluxor"
```

---

## ✅ Checkpoint

At this point you should have:

- The `Microsoft.AspNetCore.SignalR.Client` package installed
- A `HubConnection` registered as a singleton in `Program.cs`
- A `TodoHubActionBinder.cs` that maps SignalR events to Fluxor action dispatches
- The `TodoHubActionBinder` registered in DI
- The solution should build successfully: `dotnet build`

> 📝 The SignalR connection won't actually start until we call `Bind()` from a component — that's next!

---

➡️ Next: [Section 8: ToDo List Component](08-todo-list-component.md)
