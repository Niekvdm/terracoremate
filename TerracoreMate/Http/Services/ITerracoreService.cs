using Refit;
using TerracoreMate.Models.Terracore;

namespace TerracoreMate.Http.Services;

public interface ITerracoreService
{
    [Get("/player/{username}")]
    Task<Player?> GetPlayerByUsername(string username);
    
    [Get("/planets/{username}")]
    Task<PlayerPlanetResponse?> GetPlanetsByUsername(string username);
    
    [Get("/battle")]
    Task<PaginatedPlayersResponse?> GetPaginatedAttackablePlayers([Query] double maxDefense, [Query] int offset = 1, [Query] int limit = 1000);
    
    [Get("/battle_logs/{username}")]
    Task<List<BattleLog>> GetBattleLogsByUsername(string username);
    
    [Get("/claim_logs/{username}")]
    Task<List<ClaimLog>> GetClaimLogsByUsername(string username);
    
    [Get("/boss_logs/{username}")]
    Task<List<BossLog>> GetBossLogsByUsername(string username);
    
    [Get("/transactions")]
    Task<TransactionQueue> GetTransactionQueue();
}