using TerracoreMate.Enums;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to buy a crate using scrap tokens in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreContractAction.
/// </summary>
public class BuyCrateAction : TerracoreContractAction
{
    /// <summary>
    /// Initializes a new instance of the BuyCrateAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Buy Crate Action on Terracore Hive.</param>
    /// <param name="crateScrapValue">The scrap value of crate to be purchased.</param>
    public BuyCrateAction(Account account, uint crateScrapValue) : base(account)
    {
        SetContractName("tokens");
        SetContractAction("transfer");
        SetContractPayload(
            Symbol.SCRAP,
            "null",
            crateScrapValue.ToString(),
            $"{Constants.TransactionIds.Prefixes.BuyCrate}-{Guid.NewGuid()}"
        );
    }
}