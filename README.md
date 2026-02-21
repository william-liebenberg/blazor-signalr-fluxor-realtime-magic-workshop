# 🔥 Real-Time Magic: Crafting Blazor WebAssembly Apps with SignalR and Fluxor

> A hands-on workshop by **William Liebenberg**

Welcome! In this workshop you'll build a **real-time ToDo list application** from scratch — one that instantly synchronizes across every connected browser tab, phone, and device. No page refreshes required.

## What You'll Build

A fully functional ToDo app powered by:

- **Blazor WebAssembly** — interactive UI running in the browser
- **SignalR** — real-time bidirectional communication between server and clients
- **Fluxor** — predictable state management using the Flux/Redux pattern
- **.NET 10 ASP.NET Core Minimal APIs** — lightweight backend API

When you add, edit, complete, or delete a ToDo item in one browser, every other connected client sees the change instantly.

## What You'll Learn

- How to scaffold a Blazor Web App with WebAssembly interactivity
- How to create a SignalR Hub and broadcast real-time events to all connected clients
- How to build RESTful Minimal API endpoints that integrate with SignalR
- The Flux/Redux pattern and how Fluxor implements it for Blazor
- How to define Fluxor State, Actions, and Reducers for predictable state management
- How to wire SignalR events to Fluxor action dispatches so the UI updates automatically
- How to build Blazor components that re-render on state changes — without calling `StateHasChanged()`

## Prerequisites

Before the workshop begins, make sure you have the following ready:

| Tool | Version |
|------|---------|
| .NET 10 SDK | 10.0 or later |
| Git | Latest |
| IDE / Editor | Visual Studio 2026, VS Code, or JetBrains Rider |

👉 **[See Section 0 for full setup instructions](00-prerequisites.md)** — including verification commands and optional browser extensions.

## Workshop Structure

The workshop is divided into 9 hands-on sections. You'll commit your work at the end of each section so you have a clean Git history of your progress.

| # | Section | Description |
|---|---------|-------------|
| 0 | [Prerequisites & Setup](00-prerequisites.md) | Install tools and verify your environment *(do this before the workshop)* |
| 1 | [Project Scaffolding](01-project-scaffolding.md) | Create the Blazor Web App solution and initialize Git |
| 2 | [Shared Models](02-shared-models.md) | Create a shared class library with the `ToDoItem` model |
| 3 | [SignalR Hub](03-signalr-hub.md) | Build the `TodoHub` for broadcasting real-time events |
| 4 | [Backend API](04-backend-api.md) | Add an in-memory repository, Minimal API endpoints, and SignalR broadcasting |
| 5 | [Flux Pattern & Fluxor Setup](05-flux-pattern-and-fluxor-setup.md) | Learn the Flux/Redux pattern and configure Fluxor in the client |
| 6 | [State, Actions & Reducers](06-fluxor-state-actions-reducers.md) | Define the Fluxor state, actions, and pure reducer functions |
| 7 | [SignalR Client Integration](07-signalr-client-integration.md) | Wire SignalR events to Fluxor action dispatches |
| 8 | [ToDo List Component](08-todo-list-component.md) | Build the Blazor UI component that brings it all together |
| 9 | [Testing & Wrap-Up](09-testing-and-wrap-up.md) | Test real-time sync across multiple clients and celebrate! 🎉 |

## How to Follow Along

1. **Start with Section 0** — complete the prerequisites *before* the workshop
2. **Work through each section in order** — each one builds on the previous
3. **Commit at the end of each section** — every section includes a `git commit` step
4. **Check the ✅ Checkpoint** — each section ends with a checklist to verify you're on track
5. **Ask questions!** — if you get stuck, don't hesitate to ask for help

## Completed Source Code

If you fall behind or want to reference the finished application, the complete source code is available here:

🔗 **[github.com/william-liebenberg/blazor-signalr-fluxor-realtime-magic-workshop/tree/main/src/BlazinTodos](https://github.com/william-liebenberg/blazor-signalr-fluxor-realtime-magic-workshop/tree/main/src/BlazinTodos)**

---

✅ **Ready?** Head to **[Section 0: Prerequisites & Setup](workshop/00-prerequisites.md)** to get started!
