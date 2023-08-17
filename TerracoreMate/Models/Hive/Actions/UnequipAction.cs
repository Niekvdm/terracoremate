using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to unequip an item in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreHiveAction.
/// </summary>
public class UnequipAction : TerracoreHiveAction
{
    /// <summary>
    /// Initializes a new instance of the UnequipAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Unequip Action on Terracore Hive.</param>
    /// <param name="itemNumber">The number of the item to be unequipped.</param>
    public UnequipAction(Account account, uint itemNumber) : base(account, Constants.TransactionIds.Unequip, KeyType.Posting, false)
    {
        AddParameter("item_number", itemNumber);
        AddParameter("action", $"{TransactionId}-{Guid.NewGuid()}");
    }
}