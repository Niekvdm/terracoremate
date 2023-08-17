using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive;

/// <summary>
/// Represents a specific type of HiveAction tailored for Terracore transactions on the Hive blockchain network.
/// </summary>
public class TerracoreHiveAction : HiveAction
{
    /// <summary>
    /// Initializes a new instance of the TerracoreHiveAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Terracore Hive action.</param>
    /// <param name="transactionId">The unique identifier of the transaction.</param>
    /// <param name="keyType">The type of access key.</param>
    /// <param name="includeHash">A Boolean value indicating whether a unique hash should be added to the action parameters.</param>
    public TerracoreHiveAction(Account account, string transactionId, KeyType keyType, bool includeHash = true) : base(account, transactionId, keyType, "broadcast_transaction_synchronous")
    {
        if (includeHash)
            AddParameter("tx-hash", Guid.NewGuid());
    }
}