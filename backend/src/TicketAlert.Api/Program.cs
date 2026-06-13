using Microsoft.OpenApi.Models;
using TicketAlert.Api.Extensions;
using TicketAlert.Api.Middleware;

var dir = Directory.GetCurrentDirectory();
string? envPath = null;
while (dir != null)
{
    var candidate = Path.Combine(dir, ".env");
    if (File.Exists(candidate)) { envPath = candidate; break; }
    dir = Path.GetDirectoryName(dir);
}

if (envPath != null)
{
    DotNetEnv.Env.Load(envPath);
    Console.WriteLine($"Loaded .env from {envPath}");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TicketAlert API",
        Version = "v1",
        Description = "Overvåk utsolgte konserter på Ticketmaster Norge"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:Origins"] ?? "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("Frontend");
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("\nTicketAlert API running (background DB errors are expected without PostgreSQL)\n");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    var logPath = Path.Combine(AppContext.BaseDirectory, "crash.log");
    await File.WriteAllTextAsync(logPath, $"[{DateTime.UtcNow:O}] Fatal: {ex}{Environment.NewLine}");
    Console.Error.WriteLine($"\nFatal error. Details written to: {logPath}");
    Console.Error.WriteLine("Press Enter to exit...");
    Console.ReadLine();
    throw;
}
