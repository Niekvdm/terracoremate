using TerracoreMate.Enums;

namespace TerracoreMate.Models;

public class PlayerStats
{
    public double Damage { get; set; }
    public double Defense { get; set; }
    public double Engineering { get; set; }
    public double Contribute { get; set; }

    public PlayerStats(double damage, double defense, double engineering, double contribute)
    {
        Damage = damage;
        Defense = defense;
        Engineering = engineering;
        Contribute = contribute;
    }

    public double GetStatByType(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Contribute:
                return Contribute;

            case UpgradeType.Damage:
                return Damage / 10;

            case UpgradeType.Defense:
                return Defense / 10;

            case UpgradeType.Engineering:
                return Engineering;

            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
        }
    }

    public double GetStatCostByType(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            // Favor costs 1 SCRAP for 1 favor
            case UpgradeType.Contribute:
                return 1;

            case UpgradeType.Damage:
                return Math.Pow(Damage / 10, 2);

            case UpgradeType.Defense:
                return Math.Pow(Defense / 10, 2);

            case UpgradeType.Engineering:
                return Math.Pow(Engineering, 2);

            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
        }
    }

    public Dictionary<UpgradeType, double> ToLevels()
    {
        return new Dictionary<UpgradeType, double>
        {
            { UpgradeType.Contribute, Contribute },
            { UpgradeType.Damage, Damage / 10 },
            { UpgradeType.Defense, Defense / 10 },
            { UpgradeType.Engineering, Engineering },
        };
    }

    public bool CompareTo(PlayerStats stats, UpgradeType? upgradeType = null)
    {
        if (upgradeType == null)
            return stats.ToLevels().Sum(x => x.Value) > this.ToLevels().Sum(x => x.Value);

        return stats.GetStatByType(upgradeType.Value) > this.GetStatByType(upgradeType.Value);
    }
}