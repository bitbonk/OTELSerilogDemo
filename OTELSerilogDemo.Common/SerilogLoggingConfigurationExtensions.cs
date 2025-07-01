using System.Reflection;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Sinks.OpenTelemetry;
using SerilogTracing;

namespace OTELSerilogDemo.Common;

public static class SerilogLoggingConfigurationExtensions
{
    private static string ApplicationName { get; } = new Lazy<string>(() =>
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        return entryAssembly == null ? string.Empty : Path.GetFileNameWithoutExtension(entryAssembly.Location);
    }).Value;

    public static LoggerConfiguration ConfigureDefaultLogging(this LoggerConfiguration loggerConfiguration)
    {
        // TODO return value seems to be disposable, how and when dispose this?
        _ = new ActivityListenerConfiguration()
            .TraceToSharedLogger();

        return loggerConfiguration
            .Enrich.WithProperty("process.executable.name", ApplicationName)
            .Enrich.WithOpenTelemetryTraceId()
            .Enrich.WithOpenTelemetrySpanId()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.OpenTelemetry(opts =>
            {
                opts.Endpoint = "http://localhost:17011";
                opts.Protocol = OtlpProtocol.Grpc;
                opts.ResourceAttributes = new Dictionary<string, object>
                {
                    ["process.executable.name"] = ApplicationName,
                    ["service.name"] = "OTELSerilogDemo",
                    ["service.version"] = "1.0.0",
                    ["service.instance.id"] = ApplicationName
                };
            });
    }
}