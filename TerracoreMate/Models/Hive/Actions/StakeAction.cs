using TerracoreMate.Enums;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to stake a certain quantity of scrap tokens in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreContractAction.
/// </summary>
public class StakeAction : TerracoreContractAction
{
    /// <summary>
    /// Initializes a new instance of the StakeAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Stake Action on Terracore Hive.</param>
    /// <param name="scrapQuantity">The quantity of scrap tokens to be staked.</param>
    public StakeAction(Account account, uint scrapQuantity) : base(account)
    {
        SetContractName("tokens");
        SetContractAction("stake");
        SetContractPayload(
            Symbol.SCRAP,
            account.Username,
            scrapQuantity.ToString(),
            $"{Constants.TransactionIds.Prefixes.Stake}-{Guid.NewGuid()}"
        );
    }
}