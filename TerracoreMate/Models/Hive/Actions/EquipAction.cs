using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to equip an item in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreHiveAction.
/// </summary>
public class EquipAction : TerracoreHiveAction
{
    /// <summary>
    /// Initializes a new instance of the EquipAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Equip Action on Terracore Hive.</param>
    /// <param name="itemNumber">The number of the item to be equipped.</param>
    public EquipAction(Account account, uint itemNumber) : base(account, Constants.TransactionIds.Equip, KeyType.Posting, false)
    {
        AddParameter("item_number", itemNumber);
        AddParameter("action", $"{TransactionId}-{Guid.NewGuid()}");
    }
}