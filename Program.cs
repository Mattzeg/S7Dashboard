using S7Dashboard.Components;
using S7Dashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register custom services
builder.Services.AddSingleton<ConfigurationService>();
builder.Services.AddSingleton<S7CommunicationService>();
builder.Services.AddSingleton<DataPollingService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DataPollingService>());
builder.Services.AddScoped<SidebarStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
