# Section 3: SignalR Hub

> ⏱️ Estimated time: **10 minutes**

> **Recap:** In the previous section, you created the `BlazinTodos.Shared` class library and defined the `ToDoItem` model with `Id`, `Title`, and `Completed` properties. Both the server and client projects now reference this shared library.
>
> ⬅️ Previous: [Section 2: Shared Models](02-shared-models.md)

## Overview

SignalR is a library for ASP.NET Core that enables real-time, bidirectional communication between server and client. Instead of the client constantly polling the server for updates, SignalR allows the **server** to **push** updates to all connected clients instantly.

A **Hub** is the central piece of SignalR — it's a server-side class that defines methods clients can call, and methods the server can use to broadcast messages to clients.

In this section, we'll create a `TodoHub` that handles broadcasting ToDo item changes (new, updated, deleted, completed, reset) to all connected clients.

## Step 1: Create the TodoHub

ASP.NET Core WebAPI projects already include the SignalR references, so there are no additional packages to install on the server side.

In the `BlazinTodos` server project, create a new file called `TodoHub.cs`:

```csharp
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
```

Each method in this hub broadcasts a specific type of change to **all** connected clients using `Clients.All.SendAsync(...)`. The first parameter is the event name that clients will listen for (e.g., `"ReceiveNewTodo"`), and the second parameter is the data payload.

> 📝 **Note:** In a production application, you'd likely convert this to a **strongly typed hub** using an interface. For this workshop, the string-based approach keeps things simple and easy to follow.

## Step 2: Commit your changes

```powershell
git add .
git commit -m "Add SignalR hub"
```

---

## ✅ Checkpoint

At this point you should have:

- A `TodoHub.cs` file in the `BlazinTodos` server project
- The hub defines broadcast methods for all ToDo operations (add, update, delete, complete, reset)
- The solution should still build successfully: `dotnet build`

> ⚠️ The hub isn't wired up yet — we'll register it in `Program.cs` and map its endpoint in the next section.

---

➡️ Next: [Section 4: Backend API](04-backend-api.md)
