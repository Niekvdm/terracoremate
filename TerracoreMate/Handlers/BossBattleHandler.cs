using TerracoreMate.Extensions;
using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Models;
using TerracoreMate.Models.Hive.Actions;
using TerracoreMate.Models.Terracore;
using TerracoreMate.Services;

namespace TerracoreMate.Handlers;

public class BossBattleHandler : IHandler
{
    private readonly HiveService _hiveService;
    private readonly PlayerService _playerService;
    private readonly ILogger _logger;

    private Account _account;

    /// <summary>
    /// Initializes a new instance of the <see cref="BossBattleHandler"/> class.
    /// </summary>
    /// <param name="hiveService">The Hive service.</param>
    /// <param name="playerService">The Player service.</param>
    /// <param name="logger">The Logger.</param>
    public BossBattleHandler(
        HiveService hiveService,
        PlayerService playerService,
        ILogger logger
    )
    {
        _hiveService = hiveService;
        _playerService = playerService;
        _logger = logger;
    }

    /// <summary>
    /// Sets the class-level account to the provided account.
    /// </summary>
    /// <param name="account">The account to be set in the class context</param>
    public void SetAccount(Account account)
    {
        _account = account;
    }

    /// <summary>
    /// Sets the class-level account to the provided account.
    /// </summary>
    /// <param name="account">The account to be set in the class context</param>
    public async Task Fire()
    {
        if (!_account.Settings.Bosses.IsEnabled)
            return;

        var player = await _playerService.GetPlayerByUsername(_account.Username);

        if (player == null)
            return;

        if (player.Flux < _account.Settings.Bosses.FluxLimit)
            return;

        var planetsResponse = await _playerService.GetPlanetsByUsername(_account.Username);

        if (planetsResponse == null)
            return;

        foreach (var planet in planetsResponse.Planets.Where(planet =>
                     _account.Settings.Bosses.IsPlanetEnabled(planet.Name)))
        {
            LogNextBattleTime(planet);

            if (!IsEligibleToBattle(player, planet))
                continue;

            await Battle(player, planet);
        }
    }

    /// <summary>
    /// Handles the logic for boss battles asynchronously.
    /// </summary>
    /// <param name="planet">The planet where the boss is being attacked on.</param>
    /// <returns>The task instance representing the asynchronous operation.</returns>
    private async Task Battle(Player player, Planet planet)
    {
        _logger.Information("{Username}> is using {Type} on {Planet}",
            _account.Username,
            "BossBattle",
            planet.Name
        );

        var transactionId = await _hiveService.Broadcast(new BossAction(_account, planet.Name, planet.FuelCost));

        _logger.Information("{Username}> {Type} transaction #{TransactionId}",
            _account.Username,
            "BossBattle",
            transactionId
        );

        var transactionLog = await ValidateTransaction(planet);

        if (transactionLog == null)
        {
            _logger.Warning("{Username}> {Type} Failed to validate transaction #{TransactionId}",
                _account.Username,
                "BossBattle",
                transactionId
            );

            return;
        }

        _logger.Information("{Username}> {Type} on {Planet}; Rolled {Roll} with {Luck}% of luck and was {Result}",
            transactionLog.Username,
            "BossBattle",
            planet.Name,
            Math.Round(transactionLog.Roll ?? 0, 2),
            Math.Round(transactionLog.Luck ?? 0, 2),
            transactionLog.Result ? "Lucky" : "Unlucky"
        );
    }

    /// <summary>
    /// Checks if the player is eligible to battle on the specified planet.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="planet">The planet to check.</param>
    /// <returns>true if the player is eligible, false otherwise.</returns>
    private static bool IsEligibleToBattle(Player player, Planet planet)
    {
        if (player.Flux < planet.FuelCost)
            return false;

        if (planet.NextBattleDate > DateTimeOffset.UtcNow)
            return false;

        return true;
    }

    /// <summary>
    /// Logs the next battle time.
    /// </summary>
    /// <param name="player">The player to log the next battle time for.</param>
    private void LogNextBattleTime(Planet planet)
    {
        var nextBattleIn = planet.NextBattleDate.ToTimeSpanFromNow().ToClockTime();

        _logger.Information("{Username}> {Type} on {Planet} in {NextBattleIn}",
            _account.Username,
            "BossBattle",
            planet.Name,
            nextBattleIn
        );
    }

    /// <summary>
    /// Validates the transaction asynchronously.
    /// </summary>
    /// <param name="maxAttempts">The maximum number of attempts to validate the transaction.</param>
    /// <returns>The task instance representing the asynchronous operation, containing the claim log if the transaction is valid, null otherwise.</returns>
    private async Task<BossLog> ValidateTransaction(
        Planet planet,
        int maxAttempts = Constants.Application.BossTransactionValidationAttempts
    )
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            var logs = await _playerService.GetBossLogsByUsername(_account.Username);

            var log = logs.FirstOrDefault(log =>
                log.Planet.Equals(planet.Name, StringComparison.InvariantCultureIgnoreCase) &&
                log.Username.Equals(_account.Username, StringComparison.InvariantCultureIgnoreCase) &&
                log.HasOccuredWithinSeconds(60 * 2)
            );

            if (log != null)
                return log;

            // 5s delay with retry back-off
            await Task.Delay((int)(5000 + i * 0.5 * 5000));
        }

        return null;
    }
}