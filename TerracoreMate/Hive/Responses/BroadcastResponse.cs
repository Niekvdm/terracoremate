using Newtonsoft.Json;

namespace TerracoreMate.Hive.Responses;

public class BroadcastResponse
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("jsonrpc")] public string JsonRpc { get; set; }

    [JsonProperty("result")] public Result Result { get; set; }
}

public class Result
{
    [JsonProperty("block_num")] public int BlockNumber { get; set; }

    [JsonProperty("expired")] public bool IsExpired { get; set; }

    [JsonProperty("id")] public string TransactionId { get; set; }

    [JsonProperty("rc_cost")] public int ResourceCreditsCost { get; set; }

    [JsonProperty("trx_num")] public int TransactionNumber { get; set; }
}