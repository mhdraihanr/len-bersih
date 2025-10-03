using DNTCaptcha.Core;
using LenBersih.Api.Services;
using LenBersih.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// ========================================
// DATABASE CONFIGURATION
// ========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<LenBersihDbContext>(options =>
    options.UseMySQL(connectionString)
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment())
);

// ========================================
// JWT AUTHENTICATION CONFIGURATION
// ========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Remove delay of token when expires
    };
});

// ========================================
// AUTHORIZATION POLICIES
// ========================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("admin"));

    options.AddPolicy("MembersOnly", policy =>
        policy.RequireRole("members"));

    options.AddPolicy("AdminOrMembers", policy =>
        policy.RequireRole("admin", "members"));
});

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

// ========================================
// DATABASE CONNECTION TEST ON STARTUP
// ========================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LenBersihDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("🔄 Testing database connection...");
        var canConnect = await dbContext.Database.CanConnectAsync();

        if (canConnect)
        {
            logger.LogInformation("✅ Database connection successful!");

            // Log table counts for verification
            var userCount = await dbContext.Users.CountAsync();
            var groupCount = await dbContext.Groups.CountAsync();
            var reportCount = await dbContext.Reports.CountAsync();
            var statusCount = await dbContext.Statuses.CountAsync();

            logger.LogInformation("📊 Database Statistics:");
            logger.LogInformation("   • Users: {UserCount}", userCount);
            logger.LogInformation("   • Groups: {GroupCount}", groupCount);
            logger.LogInformation("   • Reports: {ReportCount}", reportCount);
            logger.LogInformation("   • Status Types: {StatusCount}", statusCount);
        }
        else
        {
            logger.LogError("❌ Database connection failed!");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Database connection error: {Message}", ex.Message);
        logger.LogError("💡 Tip: Make sure MariaDB is running (docker-compose up -d)");
    }
}

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

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

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
