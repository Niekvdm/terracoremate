using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class BossLog : BaseLog
{
    public string Username { get; set; }
    public string Planet { get; set; }
    public bool Result { get; set; }
    public double? Roll { get; set; }
    public double? Luck { get; set; }
    [JsonProperty("time")]
    public override long Timestamp { get; set; }
}