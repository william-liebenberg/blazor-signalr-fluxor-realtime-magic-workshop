# Section 1: Project Scaffolding


> ⏱️ Estimated time: **10 minutes**

> **Recap:** In the prerequisites section, you verified that your development environment is ready — .NET 9 SDK, Git, and an IDE are installed, and you optionally set up the Redux DevTools browser extension for debugging Fluxor state later.
>
> ⬅️ Previous: [Section 0: Prerequisites](00-prerequisites.md)

## Overview

In this section, we'll create the solution structure for our real-time ToDo application. We'll use the Blazor Web App template with WebAssembly interactivity, which gives us both a server-side ASP.NET Core project and a client-side Blazor WebAssembly project.

## Step 1: Create the Blazor project

Open a terminal and run the following command to scaffold a new Blazor Web App with WebAssembly interactivity enabled for all pages:

```powershell
dotnet new blazor -n BlazinTodos --interactivity WebAssembly --all-interactive
```

This creates a solution with two projects:

- **BlazinTodos** — the ASP.NET Core server project (hosts the API and serves the Blazor app)
- **BlazinTodos.Client** — the Blazor WebAssembly client project (runs in the browser)

## Step 2: Navigate into the solution folder

```powershell
cd BlazinTodos
```

## Step 3: Clean up boilerplate

The template comes with sample weather service code that we won't need. Feel free to remove the following files:

- `BlazinTodos.Client/Pages/Counter.razor`
- `BlazinTodos.Client/Pages/Weather.razor`
- Any weather-related service files

> 💡 **Tip:** Don't worry about being thorough with the cleanup — the important thing is to have a clean starting point for our ToDo application.

## Step 6: Test the app

Open the `BlazinTodos.sln` file in your IDE, and launch/debug the `BlazinTodos` server application.

Your default browser will open and load the application url (typically `https://localhost:7001`).

Click around and make sure the app is working.


## Step 5: Initialize Git

Let's set up version control so we can commit at the end of each section:

```powershell
git init
git add .
git commit -m "Initial project scaffolding"
```

---

## ✅ Checkpoint

At this point you should have:

- A `BlazinTodos` solution with two projects (`BlazinTodos` and `BlazinTodos.Client`)
- A clean Git repository with your first commit
- The app should build and run with `dotnet run --project BlazinTodos`

---

➡️ Next: [Section 2: Shared Models](02-shared-models.md)
