# Section 0: Prerequisites & Environment Setup

> ⏱️ Complete this section **before** the workshop begins.

## Required Software

Before attending the workshop, make sure you have the following installed on your machine:

| Tool         | Version       | Download                                                                                                                                                     |
| ------------ | ------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| .NET 10 SDK  | 10.0 or later | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)                                                                                       |
| Git          | Latest        | [git-scm.com](https://git-scm.com/)                                                                                                                          |
| IDE / Editor | Latest        | [Visual Studio 2026](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/) |

### Verify your setup

Open a terminal and confirm the following commands return valid output:

```powershell
dotnet --version
# Should output 10.0.x or later

git --version
# Should output git version 2.x.x
```

## Recommended Browser Extensions

- **Redux DevTools** — available for [Chrome](https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd) and [Firefox](https://addons.mozilla.org/en-US/firefox/addon/reduxdevtools/). We'll use this to inspect Fluxor state changes in real-time during the workshop.

## What We're Building

In this workshop, you'll build a real-time ToDo list application using:

- **Blazor WebAssembly** for the interactive UI
- **Fluxor** for state management within the UI application
- **SignalR** for real-time communication
- **.NET 10 ASP.NET Core — Minimal APIs** for the backend API

The goal is to keep the Blazor WebAssembly frontend synchronized across all connected clients, whether they're on mobile or desktop devices. 

By the end of the workshop, you'll have a working app where changes made on one client instantly appear on all others.

## Workshop Structure

This workshop is divided into 9 sections. At the end of each section, you'll commit your work to Git so you have a clean history of your progress:

| Section | Topic |
|---------|-------|
| 1 | Project Scaffolding |
| 2 | Shared Models |
| 3 | SignalR Hub |
| 4 | Backend API |
| 5 | Flux Pattern & Fluxor Setup |
| 6 | Fluxor State, Actions & Reducers |
| 7 | SignalR Client Integration |
| 8 | ToDo List Component |
| 9 | Testing & Wrap-Up |

---

✅ **You're ready!** Proceed to [Section 1: Project Scaffolding](01-project-scaffolding.md) when the workshop begins.
