using OTELSerilogDemo.Common;
using OTELSerilogDemo.Service;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

Log.Logger = new LoggerConfiguration()
    .ConfigureDefaultLogging()
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .SetMinimumLevel(LogLevel.Trace)
    .AddSerilog();

var host = builder.Build();
host.Run();