using TerracoreMate.Enums;
using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Models;
using TerracoreMate.Models.Hive.Actions;
using TerracoreMate.Models.Terracore;
using TerracoreMate.Services;

namespace TerracoreMate.Handlers;

public class UpgradeHandler : IHandler
{
    private readonly HiveService _hiveService;
    private readonly PlayerService _playerService;
    private readonly ILogger _logger;

    private Account _account;

    private Player? _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeHandler"/> class.
    /// </summary>
    /// <param name="hiveService">The Hive service.</param>
    /// <param name="playerService">The Player service.</param>
    /// <param name="logger">The Logger.</param>
    public UpgradeHandler(
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
    /// Asynchronously fires the upgrade handler process.
    /// </summary>
    /// <returns>The task instance representing the asynchronous operation.</returns>
    public async Task Fire()
    {
        if (!_account.Settings.Upgrades.Enabled)
            return;

        var upgradeStrategy = new RatioUpgradeStrategy
        {
            DesiredLevels = _account.Settings.Upgrades.DesiredLevels,
            DesiredRatios = _account.Settings.Upgrades.DesiredRatios
        };

        if (!upgradeStrategy.IsValid)
        {
            _logger.Error("Upgrade strategy values are invalid, aborting...");
            return;
        }

        while (true)
        {
            _player = await _playerService.GetPlayerByUsername(_account.Username);

            if (_player == null)
                break;

            var stats = PlayerService.ExtractStatsFromPlayer(_player);

            var selectedUpgradeType = upgradeStrategy.GetNextUpgrade(stats);

            if (selectedUpgradeType == null)
                break;

            var cost = (uint)stats.GetStatCostByType(selectedUpgradeType.Value);

            if (_player.HiveEngineScrap < cost)
                break;

            _player.HiveEngineScrap -= cost;

            await Upgrade(stats, selectedUpgradeType.Value, _player.HiveEngineScrap ?? 0);
        }
    }

    /// <summary>
    /// Handles the upgrade process for a given player's specific skill.
    /// Logs the upgrade action, broadcasts the action, validates the transaction, and logs the result.
    /// </summary>
    /// <param name="player">The player who's skill is to be upgraded</param>
    /// <param name="upgradeType">The type of skill to be upgraded</param>
    private async Task Upgrade(PlayerStats stats, UpgradeType upgradeType, double hiveEngineScrap)
    {
        var stat = stats.GetStatByType(upgradeType);
        var cost = (uint)stats.GetStatCostByType(upgradeType);

        var nextLevel = stat + 1;

        _logger.Information(
            "{Username}> using {Type} on {Upgrade} {From} -> {To} for {Quantity} SCRAP; Remaining {Scrap} SCRAP",
            _account.Username,
            "Upgrade",
            upgradeType.ToString(),
            stat,
            nextLevel,
            cost,
            Math.Round(hiveEngineScrap, 2)
        );

        var transactionId = await _hiveService.Broadcast(new UpgradeAction(_account, upgradeType, cost));

        _logger.Information("{Username}> {Type} {Upgrade} transaction #{TransactionId}",
            _account.Username,
            "Upgrade",
            upgradeType.ToString(),
            transactionId
        );

        var result = await ValidateTransaction(stats, upgradeType);

        if (result == null)
        {
            _logger.Warning("{Username}> {Type} {Upgrade} Failed to validate transaction #{TransactionId}",
                _account.Username,
                "Upgrade",
                upgradeType.ToString(),
                transactionId
            );

            return;
        }

        _logger.Information(
            "{Username}> {Type} {Upgrade} has succeeded, {Upgrade} is now level {Level}",
            _account.Username,
            "Upgrade",
            upgradeType.ToString(),
            upgradeType.ToString(),
            result.GetStatByType(upgradeType)
        );
    }

    /// <summary>
    /// Validates an upgrade action by checking the updated player's stats.
    /// If the stats aren't updated after a number of attempts, returns null.
    /// Each failed attempt introduces a delay before the next.
    /// </summary>
    /// <param name="stats">The stats that are to be validated</param>
    /// <param name="maxAttempts">The maximum number of validation attempts</param>
    private async Task<PlayerStats?> ValidateTransaction(
        PlayerStats stats,
        UpgradeType upgradeType,
        int maxAttempts = Constants.Application.UpgradeTransactionValidationAttempts
    )
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            _player = await _playerService.GetPlayerByUsername(_account.Username);

            var newStats = PlayerService.ExtractStatsFromPlayer(_player);

            if (stats.CompareTo(newStats, upgradeType))
                return newStats;

            // 5s delay with retry back-off
            await Task.Delay((int)(5000 + i * 0.5 * 5000));
        }

        return null;
    }
}

/// <summary>
/// Represents a strategy for performing upgrades by defined desired levels and ratio's
/// </summary>
internal class RatioUpgradeStrategy
{
    /// <summary>
    /// Desired upgrade levels by their types.
    /// </summary>
    public Dictionary<UpgradeType, double> DesiredLevels { get; set; }

    /// <summary>
    /// Desired upgrade ratios by their types.
    /// </summary>
    public Dictionary<UpgradeType, double> DesiredRatios { get; set; }

    public bool IsValid => DesiredLevels != null && DesiredLevels.Any() && DesiredRatios != null && DesiredRatios.Any();

    /// <summary>
    /// Constructor initializing both DesiredLevels and DesiredRatios properties.
    /// </summary>
    public RatioUpgradeStrategy()
    {
        DesiredLevels = new Dictionary<UpgradeType, double>();
        DesiredRatios = new Dictionary<UpgradeType, double>();
    }

    // <summary>
    /// Analyzes player's stats and calculates the next upgrade based on defined strategy.
    /// </summary>
    /// <param name="stats">Player's current statistics.</param>
    /// <returns>Upgrade type to perform next or null if upgrade should not be performed.</returns>
    public UpgradeType? GetNextUpgrade(PlayerStats stats)
    {
        var currentLevels = stats.ToLevels();
        var totalLevel = currentLevels.Values.Sum();

        if (totalLevel == 0)
        {
            return DesiredLevels.Keys.First();
        }

        var weightedRatios = CalculateWeightedRatios(currentLevels, totalLevel);

        var upgradeToPerform = DetermineUpgrade(weightedRatios);

        return ValidateUpgrade(stats, upgradeToPerform);
    }

    /// <summary>
    /// Calculates the weighted ratios for the current levels based on the total level.
    /// </summary>
    /// <param name="currentLevels">Current levels of the stats.</param>
    /// <param name="totalLevel">The total levels of the stats.</param>
    /// <returns>Weighted ratios for each upgrade type.</returns>
    private Dictionary<UpgradeType, double> CalculateWeightedRatios(Dictionary<UpgradeType, double> currentLevels,
        double totalLevel)
    {
        return DesiredLevels.Keys.ToDictionary(
            upgradeType => upgradeType,
            upgradeType => CalculateWeightedRatio(currentLevels[upgradeType], totalLevel, DesiredRatios[upgradeType]));
    }

    /// <summary>
    /// Calculates the weighted ratio for a specific level.
    /// </summary>
    /// <param name="currentLevel">The current level.</param>
    /// <param name="totalLevel">The total level.</param>
    /// <param name="desiredRatio">The desired ratio.</param>
    /// <returns>The calculated weighted ratio.</returns>
    private static double CalculateWeightedRatio(double currentLevel, double totalLevel, double desiredRatio)
    {
        var currentRatio = currentLevel / totalLevel;
        var relativeDifference = desiredRatio - currentRatio;

        return Math.Pow(relativeDifference / desiredRatio, 2);
    }

    /// <summary>
    /// Determines the type of upgrade to perform based on the weighted ratios.
    /// </summary>
    /// <param name="weightedRatios">Weighted ratios for each upgrade type.</param>
    /// <returns>The type of upgrade to perform.</returns>
    private static UpgradeType DetermineUpgrade(Dictionary<UpgradeType, double> weightedRatios)
    {
        return weightedRatios.Aggregate((highest, next) => highest.Value > next.Value ? highest : next).Key;
    }

    /// <summary>
    /// Checks if the proposed upgrade should be performed based on the current stats.
    /// </summary>
    /// <param name="stats">Current player stats.</param>
    /// <param name="upgradeToPerform">Proposed upgrade to perform.</param>
    /// <returns>Upgrade type if it is valid to perform; null otherwise.</returns>
    private UpgradeType? ValidateUpgrade(PlayerStats stats, UpgradeType upgradeToPerform)
    {
        return stats.GetStatByType(upgradeToPerform) >= DesiredLevels[upgradeToPerform] ? null : upgradeToPerform;
    }
}