using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action related to battling in the context of Terracore on the Hive blockchain network.
/// This action is a specialized form of TerracoreHiveAction.
/// </summary>
public class BattleAction : TerracoreHiveAction
{
    /// <summary>
    /// Initializes a new instance of the BattleAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Battle Action on Terracore Hive.</param>
    /// <param name="target">The target of the battle action.</param>
    public BattleAction(Account account, string target) : base(account, Constants.TransactionIds.Battle, KeyType.Posting)
    {
        AddParameter("target", target);
    }
}