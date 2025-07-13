using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Helm.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

string castleAddress = @"http://localhost:5000/";
//string castleAddress = @"https://localhost:7058/"; 
//string castleAddress = @"http://localhost:5203/";
builder.Services.AddSingleton<CommanderService>(castleUrl => new CommanderService(castleAddress));
builder.Services.AddSingleton<KnightService>(castleUrl => new KnightService(castleAddress));

await builder.Build().RunAsync();
