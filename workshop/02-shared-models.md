# Section 2: Shared Models

> ⏱️ Estimated time: **5 minutes**

> **Recap:** In the previous section, you scaffolded the Blazor Web App solution using `dotnet new blazor`, giving you two projects — `BlazinTodos` (server) and `BlazinTodos.Client` (WebAssembly client). You also cleaned up the boilerplate code and initialized a Git repository.
>
> ⬅️ Previous: [Section 1: Project Scaffolding](01-project-scaffolding.md)

## Overview

Before building the backend or frontend, we need a shared class library that both projects can reference. This library will contain the `ToDoItem` model — the data structure that flows between the API, SignalR hub, and Blazor UI.

## Step 1: Create the shared class library

From the solution root (`BlazinTodos/`), run the following commands to create the class library and wire up the project references:

```powershell
cd BlazinTodos
dotnet new classlib -n BlazinTodos.Shared

# add the Shared project to the Solution
dotnet sln add .\BlazinTodos.Shared\BlazinTodos.Shared.csproj

# add a project reference from API to Shared
dotnet add .\BlazinTodos\BlazinTodos.csproj reference .\BlazinTodos.Shared\BlazinTodos.Shared.csproj

# add a project reference from UI Client to Shared
dotnet add .\BlazinTodos.Client\BlazinTodos.Client.csproj reference .\BlazinTodos.Shared\BlazinTodos.Shared.csproj
```

## Step 2: Create the ToDoItem model

Delete the auto-generated `Class1.cs` file from `BlazinTodos.Shared`, then create a new file called `ToDoItem.cs` with the following content:

```csharp
namespace BlazinTodos.Shared;

public class ToDoItem
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Completed { get; set; } = false;
}
```

This is a simple model with three properties:

- **Id** — a unique identifier for each ToDo item
- **Title** — the text description of the task
- **Completed** — whether the task has been completed (defaults to `false`)

> 💡 **Tip:** Feel free to extend this model with additional properties like `DueDate` or `CompletedAt` if you want to experiment later.

## Step 3: Commit your changes

```powershell
git add .
git commit -m "Add shared models"
```

---

## ✅ Checkpoint

At this point you should have:

- A `BlazinTodos.Shared` class library project added to the solution
- Project references from both `BlazinTodos` and `BlazinTodos.Client` to `BlazinTodos.Shared`
- A `ToDoItem` model class in the shared project
- The solution should still build successfully: `dotnet build`

---

➡️ Next: [Section 3: SignalR Hub](03-signalr-hub.md)
