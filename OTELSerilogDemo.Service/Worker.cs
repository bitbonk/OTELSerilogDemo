namespace OTELSerilogDemo.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var contextCounter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var context = $"Context {++contextCounter}";
            _logger.LogInformation("Begin work {Context}", context);
            await DoWorkAsync(context, stoppingToken);
            _logger.LogInformation("End work {Context}", context);

            await Task.Delay(2000, stoppingToken);
        }
    }

    public async Task DoWorkAsync(string context, CancellationToken cancellationToken)
    {
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
        _logger.LogInformation("Start doing sub work {Context}", context);
        await Task.Delay(200, cancellationToken);
        _logger.LogInformation("Finished doing sub work {Context}", context);
    }
}