using Newtonsoft.Json;

namespace TerracoreMate.Models.Terracore;

public class Player
{
    [JsonProperty("username")] public string Username { get; set; }

    [JsonProperty("favor")] public double? Favor { get; set; }

    [JsonProperty("scrap")] public double? Scrap { get; set; }

    [JsonProperty("health")] public double Health { get; set; }

    [JsonProperty("damage")] public double Damage { get; set; }

    [JsonProperty("defense")] public double Defense { get; set; }

    [JsonProperty("engineering")] public double Engineering { get; set; }

    [JsonProperty("cooldown")] public long? Cooldown { get; set; }

    [JsonProperty("minerate")] public double? Minerate { get; set; }

    [JsonProperty("attacks")] public int? Attacks { get; set; }

    [JsonProperty("lastregen")] public long? Lastregen { get; set; }

    [JsonProperty("claims")] public int? Claims { get; set; }

    [JsonProperty("lastclaim")] public long? Lastclaim { get; set; }

    [JsonProperty("registrationTime")] public long RegistrationTime { get; set; }

    [JsonProperty("lastBattle")] public long? LastBattle { get; set; }

    [JsonProperty("version")] public int Version { get; set; }

    [JsonProperty("balance")] public double? Balance { get; set; }

    [JsonProperty("flux")] public double? Flux { get; set; }

    [JsonProperty("hiveEngineScrap")] public double? HiveEngineScrap { get; set; }

    [JsonProperty("hiveEngineStake")] public double? HiveEngineStake { get; set; }

    [JsonProperty("items")] public Items Items { get; set; }

    [JsonProperty("stats")] public Stats Stats { get; set; }

    [JsonProperty("lastPayout")] public long? LastPayout { get; set; }

    public bool HasNewUserProtection => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - RegistrationTime <
                                        Constants.Terracore.RegistrationProtectionTime;

    public bool HasLastBattleProtection => LastBattle.HasValue &&
                                           DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - LastBattle <
                                           Constants.Terracore.LastBattleProtectionTime;

    public DateTimeOffset? NextRegenDate => Lastregen.HasValue
        ? DateTimeOffset.FromUnixTimeMilliseconds(Lastregen.Value).AddHours(4)
        : null;
}

public class Armor
{
    [JsonProperty("item_number")] public int? ItemNumber { get; set; }

    [JsonProperty("item_id")] public int? ItemId { get; set; }

    [JsonProperty("item_equipped")] public bool ItemEquipped { get; set; }

    [JsonProperty("attributes")] public Attributes Attributes { get; set; }
}

public class Attributes
{
    [JsonProperty("dodge")] public double Dodge { get; set; }

    [JsonProperty("damage")] public double Damage { get; set; }

    [JsonProperty("defense")] public double Defense { get; set; }

    [JsonProperty("engineering")] public double Engineering { get; set; }

    [JsonProperty("crit")] public double Crit { get; set; }

    [JsonProperty("luck")] public double Luck { get; set; }
}

public class Avatar
{
    [JsonProperty("item_number")] public int? ItemNumber { get; set; }

    [JsonProperty("item_id")] public int? ItemId { get; set; }

    [JsonProperty("item_equipped")] public bool ItemEquipped { get; set; }

    [JsonProperty("attributes")] public Attributes Attributes { get; set; }
}

public class Items
{
    [JsonProperty("avatar")] public Avatar Avatar { get; set; }

    [JsonProperty("weapon")] public Weapon Weapon { get; set; }

    [JsonProperty("armor")] public Armor Armor { get; set; }

    [JsonProperty("ship")] public Ship Ship { get; set; }

    [JsonProperty("special")] public Special Special { get; set; }
}

public class Ship
{
    [JsonProperty("item_number")] public int? ItemNumber { get; set; }

    [JsonProperty("item_id")] public int? ItemId { get; set; }

    [JsonProperty("item_equipped")] public bool ItemEquipped { get; set; }

    [JsonProperty("attributes")] public Attributes Attributes { get; set; }
}

public class Special
{
    [JsonProperty("item_number")] public int? ItemNumber { get; set; }

    [JsonProperty("item_id")] public int? ItemId { get; set; }

    [JsonProperty("item_equipped")] public bool ItemEquipped { get; set; }

    [JsonProperty("attributes")] public Attributes Attributes { get; set; }
}

public class Stats
{
    [JsonProperty("damage")] public double Damage { get; set; }

    [JsonProperty("defense")] public double Defense { get; set; }

    [JsonProperty("engineering")] public double Engineering { get; set; }

    [JsonProperty("dodge")] public double Dodge { get; set; }

    [JsonProperty("crit")] public double Crit { get; set; }

    [JsonProperty("luck")] public double Luck { get; set; }
}

public class Weapon
{
    [JsonProperty("item_number")] public int? ItemNumber { get; set; }

    [JsonProperty("item_id")] public int? ItemId { get; set; }

    [JsonProperty("item_equipped")] public bool ItemEquipped { get; set; }

    [JsonProperty("attributes")] public Attributes Attributes { get; set; }
}