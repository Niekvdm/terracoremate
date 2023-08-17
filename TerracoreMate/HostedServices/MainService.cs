using TerracoreMate.Handlers;
using TerracoreMate.Handlers.Factories;
using TerracoreMate.Managers;
using TerracoreMate.Models;

namespace TerracoreMate.HostedServices;

public class MainService : IHostedService
{
    private readonly ILogger _logger;
    private readonly InstanceManager _instanceManager;

    /// <summary>
    /// The constructor of MainService class.
    /// </summary>
    /// <param name="configuration">Configuration provider, used for accessing application configuration settings.</param>
    /// <param name="handlerFactory">An instance of a class that implements the IHandlerFactory interface, used for creating handler instances.</param>
    /// <param name="logger">Logger instance used for writing log messages.</param>
    public MainService(
        IConfiguration configuration,
        IHandlerFactory handlerFactory,
        ILogger logger
    )
    {
        _logger = logger;

        _instanceManager =
            new InstanceManager(
                configuration.GetSection("Accounts").Get<Account[]>() ?? Array.Empty<Account>(),
                handlerFactory,
                logger
            );
    }

    /// <summary>
    /// Method called to start the hosted service.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token that indicates the host is stopping.</param>
    /// <returns>A task representing the start action.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Starting TerracoreMate service");

        await _instanceManager.Start();
    }

    /// <summary>
    /// Method called to stop the hosted service.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token that indicates the host is stopping.</param>
    /// <returns>A task representing the stop action.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Stopping TerracoreMate service");

        await _instanceManager.Stop();
    }
}