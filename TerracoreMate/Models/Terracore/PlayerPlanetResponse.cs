using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class PlayerPlanetResponse
{
    [JsonProperty("username")] public string Username { get; set; }
    [JsonProperty("fuel")] public double Fuel { get; set; }
    [JsonProperty("planets")] public List<Planet> Planets { get; set; }
}

public class Planet
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("lastBattle")] public long? LastBattle { get; set; }
    [JsonProperty("fuel_cost")] public int FuelCost { get; set; }
    
    public DateTimeOffset? NextBattleDate => LastBattle.HasValue
        ? DateTimeOffset.FromUnixTimeMilliseconds(LastBattle.Value).AddHours(4)
        : null;
}