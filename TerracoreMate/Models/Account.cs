using System.Text;
using TerracoreMate.Enums;
using TerracoreMate.Hive;

namespace TerracoreMate.Models;

public class Account
{
    private string _postingKey;
    private string _activeKey;
    public string Username { get; }

    public string PostingKey
    {
        set => _postingKey = value;
    }

    public string ActiveKey
    {
        set => _activeKey = value;
    }

    public Settings Settings { get; set; }

    public Account(string username, string postingKey, string activeKey)
    {
        _postingKey = postingKey;
        _activeKey = activeKey;
        Username = username;
    }

    public string GetKey(KeyType keyType = KeyType.Posting)
    {
        string key;

        switch (keyType)
        {
            case KeyType.Posting:
                key = _postingKey;
                break;

            case KeyType.Active:
                key = _activeKey;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null);
        }

        return Encoding.UTF8.GetString(Convert.FromBase64String(key));
    }
}

public class Settings
{
    public Claim Claim { get; set; }
    public Battle Battle { get; set; }
    public Upgrades Upgrades { get; set; }
    public Bosses Bosses { get; set; }
}

public class Claim
{
    public bool Enabled { get; set; }
    public double Threshold { get; set; }
}

public class Battle
{
    public bool Enabled { get; set; }
    public bool OnlyAttackIfClaimAvailable { get; set; }
}

public class Upgrades
{
    public bool Enabled { get; set; }

    /// <summary>
    /// Desired upgrade levels by their types.
    /// </summary>
    public Dictionary<UpgradeType, double> DesiredLevels { get; set; }

    /// <summary>
    /// Desired upgrade ratios by their types.
    /// </summary>
    public Dictionary<UpgradeType, double> DesiredRatios { get; set; }
}

public class Bosses
{
    public double FluxLimit { get; set; }

    public List<Planet> Planets { get; set; }

    public bool IsEnabled => Planets != null && Planets.Any(x => x.Enabled);

    public bool IsPlanetEnabled(string name) => Planets != null &&
                                                Planets.Any(x =>
                                                    x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) &&
                                                    x.Enabled);

    public record Planet(string Name, double Cost, bool Enabled);
}