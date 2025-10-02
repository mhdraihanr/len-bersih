using DNTCaptcha.Core;
using LenBersih.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

// Add CORS for Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7143",  // HTTPS Web
                "http://localhost:5247",   // HTTP Web
                "https://localhost:7083",  // HTTPS API (optional)
                "http://localhost:7083"    // HTTP API (development)
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add session services for DNTCaptcha
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Add DNTCaptcha services
builder.Services.AddDNTCaptcha(options =>
{
    options.UseSessionStorageProvider()
           .ShowThousandsSeparators(false)
           .WithEncryptionKey("LenBersih-SecureCaptchaKey-2025!")
           .InputNames(
               new DNTCaptchaComponent
               {
                   CaptchaHiddenInputName = "__RequestVerificationToken",
                   CaptchaHiddenTokenName = "__DNTCaptchaText",
                   CaptchaInputName = "DNTCaptchaInputText"
               })
           .Identifier("LenBersihCaptcha");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowBlazorWasm");

// Add session support for DNTCaptcha
app.UseSession();

// Add controllers
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
