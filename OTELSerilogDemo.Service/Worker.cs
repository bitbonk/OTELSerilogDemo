using System.Diagnostics;

namespace OTELSerilogDemo.Service;

public class Worker : BackgroundService
{
    private readonly ActivitySource _activitySource;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _activitySource = new ActivitySource("OTELSerilogDemo.Service.Worker");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var contextCounter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            // Start a new OTEL trace using System.Diagnostics.ActivitySource
            using var activity = _activitySource.StartActivity();

            var context = $"Context {++contextCounter}";
            _logger.LogInformation("Begin work {Context}", context);

            activity?.SetTag("context", context);

            await DoWorkAsync(context, stoppingToken);
            _logger.LogInformation("End work {Context}", context);

            await Task.Delay(2000, stoppingToken);
        }
    }

    public async Task DoWorkAsync(string context, CancellationToken cancellationToken)
    {
        // Create a child span for DoWorkAsync that links to the parent trace using System.Diagnostics.ActivitySource
        using var activity = _activitySource.StartActivity();
        activity?.SetTag("context", context);

        for (var i = 1; i <= 5; i++)
        {
            var newContext = $"{context}.{i}";
            _logger.LogInformation("Start doing work {Context}", newContext);
            await DoSubWorkAsync(newContext, cancellationToken);
            _logger.LogInformation("Finished doing work {Context}", newContext);
        }
    }

    public async Task DoSubWorkAsync(string context, CancellationToken cancellationToken)
    {
        // Create a child span for DoSubWorkAsync that links to the parent trace using System.Diagnostics.ActivitySource
        using var activity = _activitySource.StartActivity();
        activity?.SetTag("context", context);

        _logger.LogInformation("Start doing sub work {Context}", context);
        await Task.Delay(200, cancellationToken);
        _logger.LogInformation("Finished doing sub work {Context}", context);
    }
}