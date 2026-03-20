using E_Commerce_Web.Authentication;
using E_Commerce_Web.Components;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Authorization for Blazor components
builder.Services.AddAuthorizationCore();

# region Scoped services
builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthentication();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient();
#endregion

// JWT handler for HttpClient
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

// HttpClient for API Gateway
builder.Services.AddHttpClient("Gateway", client =>
{
	client.BaseAddress = new Uri("https://localhost:5000/api/");
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

// Razor Components
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();

// Standard middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
