using TerracoreMate.Http.Services;
using TerracoreMate.Models;
using TerracoreMate.Models.Terracore;

namespace TerracoreMate.Services;

public class PlayerService
{
    private readonly ITerracoreService _terracoreService;
    private readonly ILogger _logger;

    public PlayerService(ITerracoreService terracoreService, ILogger logger)
    {
        _terracoreService = terracoreService;
        _logger = logger;
    }

    public async Task<Player> GetPlayerByUsername(string username)
    {
        return await _terracoreService.GetPlayerByUsername(username);
    }
    
    public async Task<PlayerPlanetResponse> GetPlanetsByUsername(string username)
    {
        return await _terracoreService.GetPlanetsByUsername(username);
    }

    public async Task<PaginatedPlayersResponse> GetPaginatedAttackablePlayers(double damage)
    {
        return await _terracoreService.GetPaginatedAttackablePlayers(damage);
    }

    public async Task<List<BattleLog>> GetBattleLogsByUsername(string username)
    {
        return await _terracoreService.GetBattleLogsByUsername(username);
    }

    public async Task<List<ClaimLog>> GetClaimLogsByUsername(string username)
    {
        return await _terracoreService.GetClaimLogsByUsername(username);
    }
    
    public async Task<List<BossLog>> GetBossLogsByUsername(string username)
    {
        return await _terracoreService.GetBossLogsByUsername(username);
    }

    public static PlayerStats ExtractStatsFromPlayer(Player player)
    {
        return new PlayerStats(
            player.Damage,
            player.Defense,
            player.Engineering,
            player.Favor ?? 0
        );
    }
}