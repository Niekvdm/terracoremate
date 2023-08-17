using TerracoreMate.Enums;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to perform an upgrade using scrap tokens in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreContractAction.
/// </summary>
public class UpgradeAction : TerracoreContractAction
{
    /// <summary>
    /// Initializes a new instance of the UpgradeAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Upgrade Action on Terracore Hive.</param>
    /// <param name="upgradeType">The type of the upgrade to be performed.</param>
    /// <param name="scrapQuantity">The quantity of scrap tokens to be used for the upgrade.</param>
    public UpgradeAction(Account account, UpgradeType upgradeType, uint scrapQuantity) : base(account)
    {
        SetContractName("tokens");
        SetContractAction("transfer");
        SetContractPayload(
            Symbol.SCRAP,
            "null",
            scrapQuantity.ToString(),
            $"{Constants.TransactionIds.Prefixes.Terracore}_{upgradeType.ToString().ToLower()}-{Guid.NewGuid()}"
        );
    }
}