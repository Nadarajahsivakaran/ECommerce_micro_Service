using E_Commerce_Web.Components;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<TokenService>();
builder.Services.AddTransient<AuthMessageHandler>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("AuthApi", client =>
{
	client.BaseAddress = new Uri("https://localhost:5000/api/");
});

builder.Services.AddHttpClient("ProductApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:5000/api/");
})
.AddHttpMessageHandler<AuthMessageHandler>(); // ? chained correctly


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
