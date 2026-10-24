using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using login.web;
using login.web.services;
using login.web.MiddleWare;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Middleware HTTP handler for JWT auth
builder.Services.AddTransient<JwtAuthorizationHandler>();

//Reusable line of code so the HTTP client has the fixed base address
Action<HttpClient> configureClient = client => 
{
    // Make sure the trailing slash is included!
    client.BaseAddress = new Uri("http://localhost:5082/api/");
};

//Dependency injections for the http client with action config for URI
builder.Services.AddHttpClient<IAuthClientService, AuthClientService>(configureClient);

builder.Services.AddHttpClient<ICheckoutClientService, ImplementedCheckoutClientService>(configureClient)
.AddHttpMessageHandler<JwtAuthorizationHandler>(); //Add delegating handler

builder.Services.AddHttpClient<IDashboardClientService, DashboardClientService>(configureClient)
.AddHttpMessageHandler<JwtAuthorizationHandler>(); //adding middleware to dashboard for auth

// 1. Turns on the core authorization engine (fixes the error)
builder.Services.AddAuthorizationCore();

// 2. Tells Blazor to use your custom JWT provider instead of the default empty one
builder.Services.AddScoped<AuthenticationStateProvider, JwtStateProvider>();

await builder.Build().RunAsync();
