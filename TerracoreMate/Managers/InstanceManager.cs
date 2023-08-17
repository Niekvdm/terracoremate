using System.Collections.Concurrent;
using TerracoreMate.Handlers;
using TerracoreMate.Handlers.Factories;
using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Models;

namespace TerracoreMate.Managers;

public class InstanceManager
{
    private readonly IEnumerable<Account> _accounts;
    private readonly IHandlerFactory _handlerFactory;
    private readonly ILogger _logger;

    private Task _executionTask;
    private CancellationTokenSource _cancellationTokenSource;


    private readonly ConcurrentDictionary<string, DateTimeOffset> _lastAccountActivity;

    /// <summary>
    /// InstanceManager constructor.
    /// </summary>
    /// <param name="accounts">The accounts to manage.</param>
    /// <param name="handlerFactory">The factory to produce handlers.</param>
    /// <param name="logger">Logger used for logging events and exceptions.</param>
    public InstanceManager(IEnumerable<Account> accounts, IHandlerFactory handlerFactory, ILogger logger)
    {
        _accounts = accounts;
        _handlerFactory = handlerFactory;
        _logger = logger;

        _lastAccountActivity = new ConcurrentDictionary<string, DateTimeOffset>();
    }

    /// <summary>
    /// Starts the execution task.
    ///</summary>
    public async Task Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _executionTask = Task.Factory.StartNew(
            () => Execute(_cancellationTokenSource.Token),
            _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
        );
    }

    /// <summary>
    /// Execution task method for handling account actions.
    /// </summary>
    /// <param name="cancellationToken">Token used to signal cancellation of the task.</param>
    private async Task Execute(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await HandleAccountActions();
            await Task.Delay(Constants.Application.DefaultTaskDelay, cancellationToken);
        }
        
        _logger.Warning("Execution has finished");
    }
    
    /// <summary>
    /// Handle account actions.
    ///</summary>
    private async Task HandleAccountActions()
    {
        foreach (var account in _accounts)
        {
            if (!VerifyLastAccountActivity(account))
                continue;

            await HandleAccountAction<AttackHandler>(account);
            await HandleAccountAction<ClaimHandler>(account);
            await HandleAccountAction<UpgradeHandler>(account);
            await HandleAccountAction<BossBattleHandler>(account);

            UpdateLastActivity(account.Username);
        }
    }
    
    /// <summary>
    /// Factory method for managing account action
    /// </summary>
    /// <param name="account">The account to manage actions for.</param>
    private async Task HandleAccountAction<T>(Account account) where T : IHandler
    {
        await _handlerFactory.CreateHandler<T>(account).Fire();
    }

    /// <summary>
    /// Verifies when the last activity for a given account occurred.
    /// </summary>
    /// <param name="account">The account to check last activity for.<param>
    /// <returns>Boolean indicating if the account had recent activity or not.</returns>
    private bool VerifyLastAccountActivity(Account account)
    {
        if (_lastAccountActivity.TryGetValue(account.Username, out var lastActivity))
            if (IsRecentActivity(lastActivity))
                return false;

        return true;
    }

    /// <summary>
    /// Helper method that checks if the last account activity is recent.
    ///</summary>
    private static bool IsRecentActivity(DateTimeOffset lastActivity)
    {
        return DateTimeOffset.UtcNow < lastActivity.AddMilliseconds(Constants.Application.ActivityTimeInterval);
    }

    /// <summary>
    /// Helper method that checks if the last account activity is recent.
    ///</summary>
    private void UpdateLastActivity(string username)
    {
        _lastAccountActivity[username] = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Stops the execution task.
    ///</summary>
    public async Task Stop()
    {
        _cancellationTokenSource.Cancel();
        await _executionTask;
    }
}