# Section 8: ToDo List Component

> ⏱️ Estimated time: **15 minutes**

> **Recap:** In the previous section, you installed the SignalR client package, registered a `HubConnection` singleton in `Program.cs`, and created the `TodoHubActionBinder` — the glue that maps incoming SignalR events to Fluxor action dispatches so the UI updates automatically when other clients make changes.
>
> ⬅️ Previous: [Section 7: SignalR Client Integration](07-signalr-client-integration.md)

## Overview

This is where everything comes together! In this section, we'll build the `TodoList.razor` component that:

- Displays the list of ToDo items from Fluxor state
- Allows users to add, update, delete, complete, and reset ToDo items via HTTP calls to the API
- Automatically re-renders when the Fluxor state changes (no manual `StateHasChanged()` calls!)
- Initializes the SignalR connection via the `TodoHubActionBinder`

We'll also update `_Imports.razor` with the required using statements and add a navigation link to the menu.

## Step 1: Update _Imports.razor

Open `_Imports.razor` in the `BlazinTodos.Client` project and add the following `@using` statements so they're available across all components:

```razor
@using BlazinTodos.Shared
@using BlazinTodos.Client.Features.Todo
@using Fluxor
@using Fluxor.Blazor.Web.Components
@using Microsoft.AspNetCore.SignalR.Client
```

## Step 2: Create the TodoList component

Create a new file called `TodoList.razor` in the `BlazinTodos.Client/Pages/` folder (create the `Pages` folder if it doesn't exist).

This is the main UI component. Notice that it inherits from `FluxorComponent` — this is what enables automatic re-rendering when the injected `IState<ToDoState>` changes:

```razor
@page "/todos"

@inject IState<ToDoState> TodoState
@inject IDispatcher Dispatcher
@inject HttpClient Http
@inject HubConnection TodoHubConnection

@inherits FluxorComponent

<h3>To-Do List</h3>

<ul>
    @foreach (var todo in TodoState.Value.ToDos)
    {
        <li>
            <input type="checkbox"
		       checked="@todo.Completed"
		       @onchange="e => OnCompletedChanged(todo, e)" />

			<input type="text"
		       value="@todo.Title"
		       @oninput="e => OnTitleChanged(todo, e)" />
		       
            <button @onclick="() => DeleteToDo(todo.Id)">Delete</button>
        </li>
    }
</ul>

<input type="text" @bind="newToDoTitle" placeholder="New To-Do Title" />
<button @onclick="AddToDo">Add To-Do</button>

@code {
    private string? newToDoTitle;
    
    private Task OnCompletedChanged(ToDoItem todo, ChangeEventArgs e)
	{
	    var isChecked = e.Value is bool b && b;
	    return isChecked ? CompleteToDo(todo) : ResetToDo(todo);
	}
	
	private Task OnTitleChanged(ToDoItem todo, ChangeEventArgs e)
	{
	    todo.Title = e.Value?.ToString() ?? string.Empty;
	    return UpdateToDo(todo);
	}

    private async Task AddToDo()
    {
        var newToDo = new ToDoItem { Title = newToDoTitle };
        var response = await Http.PostAsJsonAsync("/api/todos", newToDo);
        if (response.IsSuccessStatusCode)
        {
            newToDoTitle = string.Empty;
        }
    }

    private async Task UpdateToDo(ToDoItem todo)
    {
        var response = await Http.PutAsJsonAsync($"/api/todos/{todo.Id}", todo);
    }

    private async Task CompleteToDo(ToDoItem todo)
    {
        var response = await Http.PutAsync($"/api/todos/{todo.Id}/complete", null);
    }

    private async Task ResetToDo(ToDoItem todo)
    {
        var response = await Http.PutAsync($"/api/todos/{todo.Id}/reset", null);
    }

    private async Task DeleteToDo(Guid todoId)
    {
        var response = await Http.DeleteAsync($"/api/todos/{todoId}");
    }

    protected override async Task OnInitializedAsync()
    {
	    await base.OnInitializedAsync();
	    
        var response = await Http.GetFromJsonAsync<ToDoItem[]>("/api/todos");
        if (response != null)
        {
            Dispatcher.Dispatch(new InitToDosAction(response));
        }
    }
}
```

Let's walk through the key parts:

### Injected Services

| Service | Purpose |
|---------|---------|
| `IState<ToDoState>` | Access the current ToDo list from the Fluxor store |
| `IDispatcher` | Dispatch Fluxor actions to update state |
| `HttpClient` | Make API calls to the backend |
| `HubConnection` | The SignalR connection (available for the action binder) |

### How it works

1. **On page load** (`OnInitializedAsync`): Fetches all ToDo items from the API and dispatches `InitToDosAction` to populate the Fluxor store
2. **Adding a ToDo**: Sends a **POST** request to `/api/todos`
	1. the API endpoint handles adding to the repo AND broadcasting via SignalR
	2. triggers a Fluxor **action dispatch** via the `TodoHubActionBinder`
	3. which triggers the reducer that updates the state in the store
	4. the store update then causes the component to re-render
3. **The UI never directly modifies state** — all changes go through the API → SignalR → Fluxor pipeline

> 💡 **Key insight:** Notice there are no calls to `StateHasChanged()` anywhere! Because the component inherits from `FluxorComponent`, it automatically re-renders whenever `ToDoState` changes. This is the power of combining the Flux pattern with Blazor.

## Step 3: Initialize the SignalR connection

You need to call the `TodoHubActionBinder.Bind()` method when the component initializes. Inject the binder and call `Bind()` inside `OnInitializedAsync()`:

Add to the inject statements at the top of `TodoList.razor`:

```razor
@inject TodoHubActionBinder HubActionBinder
```

Add the following line inside `OnInitializedAsync()` **before** the HTTP call:

```csharp
await HubActionBinder.Bind();
```

This starts the SignalR connection and registers all the event handlers that dispatch Fluxor actions.

## Step 4: Add navigation link

Open `Layout/NavMenu.razor` in the `BlazinTodos.Client` project and add a navigation link to the ToDo page:

```html
<div class="nav-item px-3">
	<NavLink class="nav-link" href="todos">
		<span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> ToDos
	</NavLink>
</div>
```

## Step 5: Commit your changes

```powershell
git add .
git commit -m "Add TodoList component"
```

---

## ✅ Checkpoint

At this point you should have:

- `_Imports.razor` updated with Fluxor, SignalR, and shared model using statements
- `TodoList.razor` component in the `Pages` folder
- `NavMenu.razor` updated with a link to the ToDo page
- The full application should build and run successfully!

---

➡️ Next: [Section 9: Testing & Wrap-Up](09-testing-and-wrap-up.md)
