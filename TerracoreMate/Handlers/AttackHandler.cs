using TerracoreMate.Extensions;
using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Models;
using TerracoreMate.Models.Hive.Actions;
using TerracoreMate.Models.Terracore;
using TerracoreMate.Services;

namespace TerracoreMate.Handlers;

public class AttackHandler : IHandler
{
    private readonly HiveService _hiveService;
    private readonly PlayerService _playerService;
    private readonly ILogger _logger;

    private Account _account;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttackHandler"/> class.
    /// </summary>
    /// <param name="hiveService">The Hive service.</param>
    /// <param name="playerService">The Player service.</param>
    /// <param name="logger">The Logger.</param>
    public AttackHandler(
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
    /// Asynchronously fires the attack handler process.
    /// </summary>
    /// <returns>The task instance representing the asynchronous operation.</returns>
    public async Task Fire()
    {
        if (!_account.Settings.Battle.Enabled)
            return;

        var player = await _playerService.GetPlayerByUsername(_account.Username);

        if (player == null)
            return;

        if (!IsEligibleToAttackByClaimSetting(player))
            return;

        LogNextRegenTime(player);

        if (!IsEligibleToAttackTargets(player))
            return;

        await AttackTargets(player);
    }

    /// <summary>
    /// Check if player is eligible to attack targets.
    /// </summary>
    /// <param name="player">Player instance to be checked.</param>
    /// <returns>Boolean value indicating if player is eligible for attack.</returns>
    private static bool IsEligibleToAttackTargets(Player player)
    {
        return player.Attacks > 0;
    }

    /// <summary>
    /// Logs the next regeneration time for attacks.
    /// </summary>
    /// <param name="player">Player instance which regeneration time to log.</param>
    private void LogNextRegenTime(Player player)
    {
        var nextRegenIn = player.NextRegenDate.ToTimeSpanFromNow().ToClockTime();

        _logger.Information("{Username}> has {Quantity} {Type}(s); Next recharge in {NextRegenIn}",
            _account.Username,
            player.Attacks ?? 0,
            "Attack",
            nextRegenIn
        );
    }

    /// <summary>
    /// Checks if player is eligible to attack based on the claiming setting.
    /// </summary>
    /// <param name="player">Player instance to be checked.</param>
    /// <returns>Boolean value indicating if player is eligible for attack based on claim setting.</returns>
    private bool IsEligibleToAttackByClaimSetting(Player player)
    {
        if (!_account.Settings.Battle.OnlyAttackIfClaimAvailable || player.Claims.Value > 0)
            return true;

        var nextRegenIn = player.NextRegenDate.ToTimeSpanFromNow().ToClockTime();

        _logger.Information(
            "{Username}> {Type} skipped, {Quantity} claim(s) available; Next recharge in {NextRegenIn}",
            _account.Username,
            "Attack",
            player.Claims ?? 0,
            nextRegenIn
        );

        return false;
    }

    /// <summary>
    /// Main method for processing attacks on the targets.
    /// </summary>
    /// <param name="player">Player instance which attacks to process.</param>
    private async Task AttackTargets(Player player)
    {
        var blacklist = new List<string>();

        var availableAttacks = player.Attacks!.Value;

        for (var i = 0; i < availableAttacks; i++)
        {
            var target = await GetPlayerToAttack(player.Stats.Damage, blacklist);

            if (target == null)
                continue;

            blacklist.Add(target.Username);

            _logger.Information("{Username}> is using {Type} on {Target}, Potential reward {Quantity} SCRAP",
                _account.Username,
                "Attack",
                target.Username,
                Math.Round(target.Scrap ?? 0, 4)
            );

            var transactionId = await _hiveService.Broadcast(new BattleAction(_account, target.Username));

            _logger.Information("{Username}> {Type} transaction #{TransactionId}",
                _account.Username,
                "Attack",
                transactionId
            );

            var transactionLog = await ValidateTransactionByTarget(transactionId, target.Username);

            if (transactionLog == null)
            {
                _logger.Warning("{Username}> {Type} Failed to validate transaction #{TransactionId}",
                    _account.Username,
                    "Attack",
                    transactionId
                );

                continue;
            }

            if (transactionLog.Scrap == 0)
            {
                _logger.Warning(
                    "{Username}> {Type} on {Target} resulted in {Quantity} SCRAP, Target had protection or dodged the attack",
                    transactionLog.Username,
                    "Attack",
                    transactionLog.Target,
                    Math.Round(transactionLog.Scrap ?? 0, 4)
                );

                continue;
            }

            var successRate = transactionLog.Scrap.Value / target.Scrap.Value * 100;

            _logger.Information(
                "{Username}> {Type} on {Target} resulted in {Quantity} SCRAP, Success rate {SuccessRate}%",
                transactionLog.Username,
                "Attack",
                transactionLog.Target,
                Math.Round(transactionLog.Scrap ?? 0, 4),
                successRate.ToString("0.00")
            );
        }
    }

    /// <summary>
    /// Get player to attack. Considers the player damage and blacklist.
    /// </summary>
    /// <param name="damage">Player damage used for retrieving players based on damage > defense</param>
    /// <param name="blacklist">Collection of blacklisted player usernames.</param>
    /// <returns>A Task which result is the selected player to attack or null, if no such player is found.</returns>
    private async Task<Player?> GetPlayerToAttack(double damage, ICollection<string> blacklist)
    {
        var paginatedResponse = await _playerService.GetPaginatedAttackablePlayers(damage);

        if (paginatedResponse == null || paginatedResponse.Players.Count == 0)
        {
            _logger.Warning("{Username}> no players available to {Type}",
                _account.Username,
                "Attack"
            );

            return null;
        }

        var target = paginatedResponse.Players
            .Where(x => !x.HasNewUserProtection)
            .Where(x => !x.HasLastBattleProtection)
            .Where(x => !blacklist.Contains(x.Username))
            .OrderByDescending(x => x.Scrap)
            .ThenBy(x => x.Stats.Dodge)
            .ThenBy(x => x.Defense)
            .FirstOrDefault();

        if (target == null)
            return null;

        if (!await IsTargetProtected(target.Username))
            return target;

        blacklist.Add(target.Username);
        target = await GetPlayerToAttack(damage, blacklist);

        return target;
    }

    /// <summary>
    /// Checks if the attack target is protected.
    /// </summary>
    /// <param name="target">Username of the player to verify if it's protected.</param>
    /// <returns>A Task which result is a boolean, true if the target is protected, false otherwise.</returns>
    private async Task<bool> IsTargetProtected(string target)
    {
        var logs = await _playerService.GetBattleLogsByUsername(target);

        return logs.Any(log =>
            log.Target.Equals(target, StringComparison.InvariantCultureIgnoreCase) &&
            log.HasOccuredWithinSeconds(60 * 10)
        );
    }

    /// <summary>
    /// Validates the transaction made against a target player by checking battle logs.
    /// </summary>
    /// <param name="target">Username of the player who is the target of the transaction.</param>
    /// <param name="maxAttempts">Maximum number of attempts to validate the transaction.</param>
    /// <returns>A Task which result is the battle log of the transaction or null, if validation fails after maxAttempts.</returns>
    private async Task<BattleLog?> ValidateTransactionByTarget(
        string transactionId,
        string target,
        int maxAttempts = Constants.Application.BattleTransactionValidationAttempts
    )
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            var logs = await _playerService.GetBattleLogsByUsername(_account.Username);

            var log = logs.FirstOrDefault(log =>
                log.Username.Equals(_account.Username, StringComparison.InvariantCultureIgnoreCase) &&
                log.Target.Equals(target, StringComparison.InvariantCultureIgnoreCase) &&
                log.TransactionId.Equals(transactionId, StringComparison.InvariantCultureIgnoreCase)
            );

            if (log != null)
                return log;

            // 5s delay with retry back-off
            await Task.Delay((int)(5000 + i * 0.5 * 5000));
        }

        return null;
    }
}