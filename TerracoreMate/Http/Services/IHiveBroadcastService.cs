using TerracoreMate.Hive.Responses;
using TerracoreMate.Http.Attributes;
using TerracoreMate.Models.Hive;

namespace TerracoreMate.Http.Services;

public interface IHiveBroadcastService
{
    [HivePost]
    Task<BroadcastResponse> Broadcast([Hive] HiveAction action);
}