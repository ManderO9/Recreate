using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recreate;

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
