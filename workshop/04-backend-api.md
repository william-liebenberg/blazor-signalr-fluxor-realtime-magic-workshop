# Section 4: Backend API

> ⏱️ Estimated time: **15 minutes**

> **Recap:** In the previous section, you created the `TodoHub` — a SignalR Hub with broadcast methods for each ToDo operation (add, update, delete, complete, reset). The hub isn't wired up yet — that happens in this section.
>
> ⬅️ Previous: [Section 3: SignalR Hub](03-signalr-hub.md)

## Overview

With the SignalR Hub in place, we now need to build the backend that actually manages our ToDo data. In this section we'll create:

1. A **ToDoRepository** — a simple in-memory repository for CRUD operations
2. **Service registration** — configuring SignalR and the repository in `Program.cs`
3. **Minimal API endpoints** — RESTful endpoints that perform CRUD operations and broadcast changes via SignalR

The key pattern here is: every API endpoint that modifies data will also **broadcast the change via SignalR** to all connected clients. This is what makes the app real-time.

## Step 1: Create the ToDoRepository

In the `BlazinTodos` server project, create a new file called `ToDoRepository.cs`. This is a simple in-memory store seeded with a few starter items:

```csharp
using Shared;

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
```

> 💡 **Note:** We're using an in-memory list for simplicity. In a real application, you'd replace this with a database-backed repository. Because it's registered as a singleton (next step), the data persists for the lifetime of the server process.

## Step 2: Configure services in Program.cs

Open the `Program.cs` file in the `BlazinTodos` server project. Add the SignalR services and register the `ToDoRepository` as a singleton:

```csharp
using BlazinTodos;
using BlazinTodos.Client.Pages;
using BlazinTodos.Components;

using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// ⬇️ Register SignalR ⬇️
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});
// ⬆️ ---------------- ⬆️ //

// ⬇️ Register custom ToDo repository for basic CRUD operations ⬇️
builder.Services.AddSingleton<ToDoRepository>();
// ⬆️ --------------------------------------------------------- ⬆️ //

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// ⬇️ Map SignalR Hub ⬇️
app.MapHub<TodoHub>("/todoHub");
// ⬆️ --------------- ⬆️ //
```

Key things to note:

- **`AddSignalR()`** registers the SignalR services in the dependency injection container
- **`AddResponseCompression()`** optimizes SignalR's WebSocket communication
- **`AddSingleton<ToDoRepository>()`** ensures a single instance of our repository is shared across all requests
- **`MapHub<TodoHub>("/todoHub")`** exposes the SignalR hub at the `/todoHub` endpoint

## Step 3: Add the Minimal API endpoints

Still in `Program.cs`, add the following API endpoints **after** the `MapHub` line but **before** `app.Run()`. Each endpoint performs a CRUD operation and then broadcasts the change to all connected clients via `IHubContext<TodoHub>`.

We inject `IHubContext<TodoHub>` rather than the `TodoHub` directly because Hub instances are transient — they're created per-call/per-request and disposed immediately. 

`IHubContext` gives us access to the hub's client-communication capabilities from outside the hub itself, which is exactly what we need in our API endpoints.

```csharp

app.MapGet("/api/todos", (ToDoRepository repo) => repo.GetAllTodos());

app.MapPost("/api/todos", async (ToDoRepository repo, IHubContext<TodoHub> hubContext, ToDoItem newToDo) =>
{
    await repo.AddTodoAsync(newToDo);
    await hubContext.Clients.All.SendAsync("ReceiveNewTodo", newToDo);
    return Results.Created($"/todos/{newToDo.Id}", newToDo);
});

app.MapPut("/api/todos/{id}", async (ToDoRepository repo, IHubContext<TodoHub> hubContext, Guid id, ToDoItem updatedToDo) =>
{
    var result = await repo.UpdateTodoAsync(id, updatedToDo);
    if (result is not null)
    {
        await hubContext.Clients.All.SendAsync("ReceiveUpdatedTodo", result);
        return Results.Ok(result);
    }
    return Results.NotFound();
});

app.MapDelete("/api/todos/{id}", async (ToDoRepository repo, IHubContext<TodoHub> hubContext, Guid id) =>
{
    var result = await repo.DeleteTodoAsync(id);
    if (result)
    {
        await hubContext.Clients.All.SendAsync("ReceiveDeletedTodo", id);
        return Results.Ok();
    }
    return Results.NotFound();
});

// Dedicated endpoint for completing a ToDo
app.MapPut("/api/todos/{id}/complete", async (ToDoRepository repo, IHubContext<TodoHub> hubContext, Guid id) =>
{
    var todo = await repo.GetTodoAsync(id);
    if (todo == null) return Results.NotFound();

    todo.Completed = true;
    var result = await repo.UpdateTodoAsync(id, todo);
    if (result is not null)
    {
        await hubContext.Clients.All.SendAsync("ReceiveCompletedTodo", result);
        return Results.Ok(result);
    }

    return Results.NotFound();
});

app.MapPut("/api/todos/{id}/reset", async (ToDoRepository repo, IHubContext<TodoHub> hubContext, Guid id) =>
{
    var todo = await repo.GetTodoAsync(id);
    if (todo == null) return Results.NotFound();

    todo.Completed = false;
    var result = await repo.UpdateTodoAsync(id, todo);
    if (result is not null)
    {
        await hubContext.Clients.All.SendAsync("ReceiveResettedTodo", result);
        return Results.Ok(result);
    }

    return Results.NotFound();
});
```

Make sure `Program.cs` ends with:

```csharp
app.Run();
```

Notice the pattern in each endpoint:
1. **Perform the operation** on the repository
2. **Broadcast the change** via `hubContext.Clients.All.SendAsync(...)` using the same event names defined in our `TodoHub`
3. **Return an appropriate HTTP response**

## Step 4: Add API Documentation with SwaggerUI & Scalar

Now that we have working API endpoints, let's add interactive documentation so we can easily explore and test them. We'll set up both **SwaggerUI** (the classic) and **Scalar** (the modern alternative) — both consume the same OpenAPI document.

### Install the NuGet packages

Run the following commands from the `BlazinTodos` server project directory:

```powershell
cd .\BlazinTodos

# Add the Swagger and Scalar API documentation packages
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
dotnet add package Scalar.AspNetCore
```

- **`Microsoft.AspNetCore.OpenApi`** — generates the OpenAPI document describing your API
- **`Swashbuckle.AspNetCore.SwaggerUI`** — serves the classic Swagger UI
- **`Scalar.AspNetCore`** — serves the modern Scalar API reference UI

### Register OpenAPI services

In `Program.cs`, add the OpenAPI service registration alongside the other services:

```csharp
// ⬇️ Register OpenAPI document generation ⬇️
builder.Services.AddOpenApi();
// ⬆️ ------------------------------------ ⬆️ //
```

### Map the documentation middleware

Still in `Program.cs`, add the following **inside** the existing `if (app.Environment.IsDevelopment())` block so the docs are only available during development:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    // ⬇️ API documentation ⬇️
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BlazinTodos API");
    });
    app.MapScalarApiReference();
    // ⬆️ ----------------- ⬆️ //
}
```

## Step 5 - Try it out

Run the app and navigate to:
- **SwaggerUI:** [https://localhost:7001/swagger](https://localhost:7001/swagger)
- **Scalar:** [https://localhost:7001/scalar/v1](https://localhost:7001/scalar/v1)

Both UIs let you send requests directly to your API — try creating, completing, and deleting ToDo items!

## Step 6: Commit your changes

```powershell
git add .
git commit -m "Add backend API with SignalR broadcasting, SwaggerUI & Scalar"
```

---

## ✅ Checkpoint

At this point you should have:

- A `ToDoRepository.cs` with in-memory CRUD operations and seed data
- `Program.cs` configured with SignalR services, the hub endpoint, Minimal API routes, and API documentation
- All API routes use the `/api/todos` prefix
- SwaggerUI available at `/swagger` and Scalar at `/scalar/v1` (development only)
- The solution should build successfully: `dotnet build`

> 💡 Use **SwaggerUI** or **Scalar** to interactively test your API endpoints — they're the easiest way to verify everything is working before we build the frontend.

---

➡️ Next: [Section 5: Flux Pattern & Fluxor Setup](05-flux-pattern-and-fluxor-setup.md)
