// Source: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/

using Microsoft.Extensions.Options;

public class BackgroundJob : BackgroundService
{
    private readonly IOptionsMonitor<ExternalApiSettings> _optionsMonitor;
    private readonly ILogger<BackgroundJob> _logger;
    private IDisposable _changeListener = null!;

    public BackgroundJob(
        IOptionsMonitor<ExternalApiSettings> optionsMonitor,
        ILogger<BackgroundJob> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _changeListener = _optionsMonitor
            ?.OnChange(x => _logger.LogInformation(
                "ExternalApiSettings changed! New value: {externalApiSettings}",
                System.Text.Json.JsonSerializer.Serialize(x)));

        return Task.CompletedTask;
    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _changeListener?.Dispose();

        base.StopAsync(cancellationToken);

        return Task.CompletedTask;
    }
}