using BlazinTodos;
using BlazinTodos.Client.Pages;
using BlazinTodos.Components;
using BlazinTodos.Shared;

using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;

using Scalar.AspNetCore;

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

// ⬇️ Register OpenAPI document generation ⬇️
builder.Services.AddOpenApi();
// ⬆️ ------------------------------------ ⬆️ //

var app = builder.Build();

// Configure the HTTP request pipeline.
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


// ⬇️ Map endpoints to interact with the TodoRepository ⬇️
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

// ⬆️ ---------------------------------- ⬆️ //

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazinTodos.Client._Imports).Assembly);

app.Run();
