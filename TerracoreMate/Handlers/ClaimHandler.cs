using TerracoreMate.Extensions;
using TerracoreMate.Handlers.Interfaces;
using TerracoreMate.Models;
using TerracoreMate.Models.Hive.Actions;
using TerracoreMate.Models.Terracore;
using TerracoreMate.Services;

namespace TerracoreMate.Handlers;

public class ClaimHandler : IHandler
{
    private readonly HiveService _hiveService;
    private readonly PlayerService _playerService;
    private readonly ILogger _logger;

    private Account _account;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimHandler"/> class.
    /// </summary>
    /// <param name="hiveService">The Hive service.</param>
    /// <param name="playerService">The Player service.</param>
    /// <param name="logger">The Logger.</param>
    public ClaimHandler(
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
        if (!_account.Settings.Claim.Enabled)
            return;

        var player = await _playerService.GetPlayerByUsername(_account.Username);

        if (player == null)
            return;

        LogNextRegenTime(player);

        if (!IsEligibleToClaimScrap(player))
            return;

        await ClaimScrap(player);
    }

    /// <summary>
    /// Handles the logic for claiming scrap asynchronously.
    /// </summary>
    /// <param name="player">The player who is claiming the scrap.</param>
    /// <returns>The task instance representing the asynchronous operation.</returns>
    private async Task ClaimScrap(Player player)
    {
        if (!player.Scrap.HasValue || player.Scrap == 0)
            return;

        _logger.Information("{Username}> is claiming {Quantity} {Type}",
            _account.Username,
            Math.Round(player.Scrap ?? 0, 4),
            "SCRAP"
        );

        var transactionId = await _hiveService.Broadcast(new ClaimAction(_account, player.Scrap.Value));

        _logger.Information("{Username}> {Type} transaction #{TransactionId}",
            _account.Username,
            "Claim",
            transactionId
        );

        var transactionLog = await ValidateTransaction();

        if (transactionLog == null)
        {
            _logger.Warning("{Username}> {Type} Failed to validate transaction #{TransactionId}",
                _account.Username,
                "Claim",
                transactionId
            );

            return;
        }

        _logger.Information("{Username}> {Type} succeeded, {Quantity} SCRAP has been claimed",
            transactionLog.Username,
            "Claim",
            Math.Round(transactionLog.Scrap ?? 0, 4)
        );
    }

    /// <summary>
    /// Checks if the player is eligible to claim scrap.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>true if the player is eligible, false otherwise.</returns>
    private bool IsEligibleToClaimScrap(Player player)
    {
        if (player.Claims == null || player.Claims == 0)
            return false;

        if (!player.Scrap.HasValue || player.Scrap < _account.Settings.Claim.Threshold)
        {
            _logger.Information("{Username}> {Type} threshold {Quantity} / {Threshold} SCRAP not yet exceeded",
                _account.Username,
                "Claim",
                Math.Round(player.Scrap ?? 0, 4),
                Math.Round(_account.Settings.Claim.Threshold, 4)
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Logs the next regeneration time.
    /// </summary>
    /// <param name="player">The player to log the next regeneration time for.</param>
    private void LogNextRegenTime(Player player)
    {
        var nextRegenIn = player.NextRegenDate.ToTimeSpanFromNow().ToClockTime();

        _logger.Information("{Username}> {Type} skipped, {Quantity} available; Next recharge in {NextRegenIn}",
            _account.Username,
            "Claim",
            player.Claims ?? 0,
            nextRegenIn
        );
    }

    /// <summary>
    /// Validates the transaction asynchronously.
    /// </summary>
    /// <param name="maxAttempts">The maximum number of attempts to validate the transaction.</param>
    /// <returns>The task instance representing the asynchronous operation, containing the claim log if the transaction is valid, null otherwise.</returns>
    private async Task<ClaimLog?> ValidateTransaction(
        int maxAttempts = Constants.Application.ClaimTransactionValidationAttempts
    )
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            var logs = await _playerService.GetClaimLogsByUsername(_account.Username);

            var log = logs.FirstOrDefault(log =>
                log.Username.Equals(_account.Username, StringComparison.InvariantCultureIgnoreCase) &&
                log.HasOccuredWithinSeconds(60 * 5)
            );

            if (log != null)
                return log;

            // 5s delay with retry back-off
            await Task.Delay((int)(5000 + i * 0.5 * 5000));
        }

        return null;
    }
}