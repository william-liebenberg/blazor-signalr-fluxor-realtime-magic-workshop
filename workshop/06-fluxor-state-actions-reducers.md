# Section 6: Fluxor State, Actions & Reducers

> ⏱️ Estimated time: **15 minutes**

> **Recap:** In the previous section, you learned about the Flux/Redux unidirectional data flow pattern and how Fluxor implements it for Blazor. You installed the Fluxor NuGet packages, configured Fluxor services and Redux DevTools in `Program.cs`, and added the `StoreInitializer` to `Routes.razor`.
>
> ⬅️ Previous: [Section 5: Flux Pattern & Fluxor Setup](05-flux-pattern-and-fluxor-setup.md)

## Overview

Now it's time to define the core Fluxor building blocks for our ToDo feature. We'll create:

- **State** — the data structure that holds our list of ToDo items
- **Feature** — tells Fluxor about our state and provides its initial value
- **Actions** — plain records that describe what happened (e.g., "a ToDo was added")
- **Reducers** — pure functions that take the current state + an action and return a new state

All of these files will live in a `Features/ToDo/` folder inside the `BlazinTodos.Client` project.

## Step 1: Create the folder structure

In the `BlazinTodos.Client` project, create the following folder structure:

```
BlazinTodos.Client/
  Features/
    ToDo/
```

## Step 2: Create the ToDoState

Create `Features/ToDo/ToDoState.cs`. This is an immutable record that holds the current list of ToDo items:

```csharp
using BlazinTodos.Shared;

namespace BlazinTodos.Client.Features.Todo;

public record ToDoState
{
    public List<ToDoItem> ToDos { get; init; } = [];
}
```

Using a `record` with `init`-only properties ensures state is immutable — you can't modify it directly, only create a new instance with updated values using the `with` expression.

## Step 3: Create the Feature class

Create `Features/ToDo/Feature.cs`. This class tells Fluxor about our state — its name and initial value:

```csharp
public class Feature : Feature<ToDoState>
{
    public override string GetName() => "ToDo";
    protected override ToDoState GetInitialState() => new();
}
```

> 📝 **Note:** You'll need to add the appropriate `using` statements for `Fluxor` and the `BlazinTodos.Client.Features.Todo` namespace.

## Step 4: Define the Actions

Create `Features/ToDo/Actions.cs`. Each action is a simple record that carries the data needed for a specific operation:

```csharp
using BlazinTodos.Shared;
using Fluxor;

namespace BlazinTodos.Client.Features.ToDo;

public record InitToDosAction(ToDoItem[] NewToDos);
public record AddToDoAction(ToDoItem NewToDo);
public record UpdateToDoAction(ToDoItem UpdatedToDo);
public record DeleteToDoAction(Guid TodoId);
public record CompleteToDoAction(ToDoItem CompletedToDo);
public record ResettedToDoAction(ToDoItem ResettedToDo);
```

Notice how each action is just a data container — it describes the **intent** of the state change without specifying **how** the state should change. That's the **reducer's** job.

## Step 5: Create the Reducers

Create `Features/ToDo/Reducers.cs`. Reducers are **pure functions** that take the current state and an action, then return a **new** state. They use the **`with`** expression to create modified copies of the state record:

```csharp
using BlazinTodos.Shared;
using Fluxor;

namespace BlazinTodos.Client.Features.ToDo;

public static class ToDoReducers
{
	// Add the NewToDo to the existing state.ToDos
    [ReducerMethod]
    public static ToDoState ReduceAddToDoAction(ToDoState state, AddToDoAction action) =>
        state with { ToDos = new List<ToDoItem>(state.ToDos) { action.NewToDo } };

	// find the targe todo item via id lookup, and return the updated copy (new state from the action)
    [ReducerMethod]
    public static ToDoState ReduceUpdateToDoAction(ToDoState state, UpdateToDoAction action) =>
        state with { ToDos = state.ToDos.Select(todo => todo.Id == action.UpdatedToDo.Id ? action.UpdatedToDo : todo).ToList() };

	// filter out the item that needs to be deleted
    [ReducerMethod]
    public static ToDoState ReduceDeleteToDoAction(ToDoState state, DeleteToDoAction action) =>
        state with { ToDos = state.ToDos.Where(todo => todo.Id != action.TodoId).ToList() };

	// find the target todo item via id lookup, and return the completed copy (new state from the action)
    [ReducerMethod]
    public static ToDoState ReduceCompleteToDoAction(ToDoState state, CompleteToDoAction action) =>
        state with { ToDos = state.ToDos.Select(todo => todo.Id == action.CompletedToDo.Id ? action.CompletedToDo : todo).ToList() };

	// find the target todo item via id lookup, and return the resetted copy (new state from the action)
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
```

Let's break down what each reducer does:

- **AddToDo** — creates a new list that includes all existing items plus the new one
- **UpdateToDo** — maps over the list, replacing the matching item with the updated version
- **DeleteToDo** — filters out the item with the matching ID
- **CompleteToDo** — replaces the matching item with its completed version
- **ResettedToDo** — replaces the matching item with its reset version

> ⚠️ **Important:** You also need a reducer for the `InitToDosAction` that we'll dispatch when the component first loads to populate the list from the API. Add the following reducer to the `ToDoReducers` class:
>
> ```csharp
> [ReducerMethod]
> public static ToDoState ReduceInitToDosAction(ToDoState state, InitToDosAction action) =>
>     state with { ToDos = new List<ToDoItem>(action.NewToDos) };
> ```

## Step 6: Commit your changes

```powershell
git add .
git commit -m "Add Fluxor state, actions, and reducers"
```

---

## ✅ Checkpoint

At this point you should have:

- `Features/ToDo/` folder in the client project with four files:
  - `ToDoState.cs` — the state record
  - `Feature.cs` — the Fluxor feature definition
  - `Actions.cs` — all action records
  - `Reducers.cs` — all reducer methods (including `InitToDosAction`)
- The solution should build successfully: `dotnet build`

---

➡️ Next: [Section 7: SignalR Client Integration](07-signalr-client-integration.md)
