using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to claim an amount in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreHiveAction.
/// </summary>
public class ClaimAction : TerracoreHiveAction
{
    /// <summary>
    /// Initializes a new instance of the ClaimAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Claim Action on Terracore Hive.</param>
    /// <param name="amount">The amount to be claimed.</param>
    public ClaimAction(Account account, double amount) : base(account, Constants.TransactionIds.Claim, KeyType.Posting)
    {
        AddParameter("amount", amount);
    }
}