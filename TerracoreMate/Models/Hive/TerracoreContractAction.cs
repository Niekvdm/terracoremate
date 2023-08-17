using TerracoreMate.Enums;
using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive;

/// <summary>
/// An abstract class that inherits from HiveAction and provides utility specific to Terracore contracts.
/// </summary>
public abstract class TerracoreContractAction : HiveAction
{
    /// <summary>
    /// Initializes a new instance of the TerracoreContractAction class with the specified account.
    /// The transaction ID and key type are set to predefined constants, and the method to broadcast transaction.
    /// </summary>
    /// <param name="account">The account associated with the Terracore contract action.</param>
    public TerracoreContractAction(Account account) : base(account,
        Constants.TransactionIds.HiveMainTransaction, KeyType.Active, "broadcast_transaction_synchronous")
    {
    }

    /// <summary>
    /// Sets the name of the contract.
    /// </summary>
    /// <param name="contractName">The name of the contract.</param>
    public void SetContractName(string contractName)
    {
        AddParameter("contractName", contractName);
    }

    /// <summary>
    /// Sets the action to be performed on the contract.
    /// </summary>
    /// <param name="contractAction">The action to be performed.</param>
    public void SetContractAction(string contractAction)
    {
        AddParameter("contractAction", contractAction);
    }
    
    /// <summary>
    /// Sets the payload for the contract.
    /// </summary>
    /// <param name="symbol">The symbol (presumably a type of token or currency) involved in the contract.</param>
    /// <param name="to">The recipient of the quantity specified.</param>
    /// <param name="quantity">The amount of the symbol to be transferred.</param>
    /// <param name="memo">A note or memo to be included with the contract.</param>
    public void SetContractPayload(Symbol symbol, string to, string quantity, object memo)
    {
        AddParameter("contractPayload", new Dictionary<string, object>
        {
            { "symbol", symbol.ToString() },
            { "to", to },
            { "quantity", quantity },
            { "memo", memo },
        });
    }
}