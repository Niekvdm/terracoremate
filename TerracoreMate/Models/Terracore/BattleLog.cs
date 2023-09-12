using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class BattleLog : BaseLog
{
    public string Username { get; set; }
    [JsonProperty("attacked")]
    public string Target { get; set; }
    public double? Scrap { get; set; }
    public string Seed { get; set; }
    public double Roll { get; set; }

    public string TransactionId
    {
        get
        {
            var seedParts = Seed?.Split('@');
            
            if(seedParts == null) 
                return string.Empty;

            var index = Constants.Terracore.SeedTransactionIdIndex;
            
            return seedParts.Length > index ? seedParts[index] : string.Empty;
        }
    }
}