var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/", () => "Hello from inside a Docker container!");
app.MapGet("/api/time", () => new { time = DateTime.Now, machine = Environment.MachineName });

app.Run();