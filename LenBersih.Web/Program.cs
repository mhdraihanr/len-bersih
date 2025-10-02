using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LenBersih.Web;
using DNTCaptcha.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API calls
// Development: Use HTTP endpoint (port 7083)
// Production: Use HTTPS endpoint
#if DEBUG
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:7083/") });
#else
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7083/") });
#endif

// DNTCaptcha.Blazor components are available for use

await builder.Build().RunAsync();
