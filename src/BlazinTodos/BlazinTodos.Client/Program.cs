// ⬇️ Add the using statement to get access to Fluxor
using BlazinTodos.Client;

using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
// ⬆️

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ⬇️ Add HTTP Client to the defualt backend Web API
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });
// ⬆️

// ⬇️ Add Fluxor and Redux DevTools
builder.Services.AddFluxor(options =>
{
    options
      .ScanAssemblies(typeof(Program).Assembly)
      //.UseRouting() // TODO: Do we need routing?
      .UseReduxDevTools();
});
// ⬆️

builder.Services.AddSingleton<HubConnection>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/todoHub"))
        .WithAutomaticReconnect()
        .Build();
});

builder.Services.AddScoped<TodoHubActionBinder>();

await builder.Build().RunAsync();
