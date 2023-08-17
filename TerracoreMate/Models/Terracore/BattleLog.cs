using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class BattleLog : BaseLog
{
    public string Username { get; set; }
    [JsonProperty("attacked")]
    public string Target { get; set; }
    public double? Scrap { get; set; }
}