using MudBlazor.Services;
using Helm.Client.Pages;
using Helm.Components;
using Helm.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

string castleAddress = @"http://localhost:5000/";
//string castleAddress = @"https://localhost:7058/"; 
//string castleAddress = @"http://localhost:5203/";
builder.Services.AddSingleton<CommanderService>(castleUrl => new CommanderService(castleAddress));
builder.Services.AddSingleton<KnightService>(castleUrl => new KnightService(castleAddress));

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

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Helm.Shared._Imports).Assembly);
//.AddAdditionalAssemblies(typeof(Helm.Client._Imports).Assembly);

app.Run();
