
namespace MinimalApiDemo.Settings;

public class AppSettings
{
    public string AppName { get; set; } = "Minimal API Demo";
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 50;
    public string ApiKey { get; set; } = string.Empty;
}
