using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class TransactionQueue
{
    [JsonProperty("transactions")]
    public int Transactions { get; set; }
}