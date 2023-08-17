using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class ClaimLog : BaseLog
{
    public string Username { get; set; }
    
    [JsonProperty("qty")]
    public double? Scrap { get; set; }
    
    [JsonProperty("time")]
    public override long Timestamp { get; set; }
}