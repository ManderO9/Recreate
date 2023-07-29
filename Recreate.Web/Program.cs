using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recreate.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// If we are in production
if(builder.HostEnvironment.IsProduction())
{
    // Change the route prefix
    Routes.RoutePrefix = "/Recreate";
}

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();


// TODO: start working on boids
// TODO: add credits to the guy in the credits page