using TerracoreMate.Enums;
using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to perform an upgrade using scrap tokens in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreContractAction.
/// </summary>
public class BossAction : TerracoreContractAction
{
    /// <summary>
    /// Initializes a new instance of the BossAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Planet Action on Terracore Hive.</param>
    /// <param name="planet">The name of the planet to be battle on.</param>
    /// <param name="fluxQuantity">The quantity of flux tokens to be used as fuel.</param>
    public BossAction(Account account, string planet, double fluxQuantity) : base(account)
    {
        SetContractName("tokens");
        SetContractAction("transfer");
        SetContractPayload(
            
            Symbol.FLUX,
            "null",
            fluxQuantity.ToString(),
            new
            {
                hash = $"{Constants.TransactionIds.Prefixes.BossFight}-{Guid.NewGuid()}",
                planet = planet
            }
        );
    }
}