using TerracoreMate.Http.Services;
using TerracoreMate.Models.Hive;

namespace TerracoreMate.Services;

public class HiveService
{
    private readonly IHiveGlobalService _hiveGlobalService;
    private readonly IHiveBroadcastService _hiveBroadcastService;
    private readonly ILogger _logger;

    public HiveService(IHiveGlobalService hiveGlobalService, IHiveBroadcastService hiveBroadcastService, ILogger logger)
    {
        _hiveGlobalService = hiveGlobalService;
        _hiveBroadcastService = hiveBroadcastService;
        _logger = logger;
    }

    public async Task<string> Broadcast(HiveAction action)
    {
        var response = await _hiveBroadcastService.Broadcast(action);

        return response.Result.TransactionId;
    }
}