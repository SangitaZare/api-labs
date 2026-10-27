using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using ElasticLoggingApi.Endpoints;
using Serilog;
using Serilog.Events;

// If logs do not reach Elasticsearch, Serilog prints the reason here
Serilog.Debugging.SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);

var endpoint = builder.Configuration["Elastic:Endpoint"];
var apiKey = builder.Configuration["Elastic:ApiKey"];
var dataset = builder.Configuration["Elastic:Dataset"] ?? "elasticloggingapi";
var ns = builder.Configuration["Elastic:Namespace"] ?? "demo";

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "ElasticLoggingApi")
        .WriteTo.Console();

    if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey))
    {
        configuration.WriteTo.Elasticsearch(new[] { new Uri(endpoint) }, opts =>
        {
            opts.DataStream = new DataStreamName("logs", dataset, ns);
            opts.BootstrapMethod = BootstrapMethod.None;

            opts.ConfigureChannel = channelOpts =>
            {
                channelOpts.ExportExceptionCallback = ex =>
                    Console.Error.WriteLine("ELASTIC EXPORT EXCEPTION: " + ex);

              
            };
        }, transport =>
        {
            transport.Authentication(new ApiKey(apiKey));
            transport.DisablePing();
        });
    }
    else
    {
        Console.WriteLine("WARNING: Elastic:Endpoint or Elastic:ApiKey is missing. Logging to the console only.");
    }
});

var app = builder.Build();

app.UseSerilogRequestLogging();     // one log line per HTTP request

app.MapGet("/", () => "Elastic Logging API is running");
app.MapDemoEndpoints();

app.Logger.LogInformation("Application started in {EnvironmentName}", app.Environment.EnvironmentName);

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();            // sends the last logs before the app closes
}