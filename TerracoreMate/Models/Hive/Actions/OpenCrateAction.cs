using TerracoreMate.Enums;
using TerracoreMate.Hive;

namespace TerracoreMate.Models.Hive.Actions;

/// <summary>
/// Represents a specific action to open a crate in the context of Terracore on the Hive blockchain.
/// This action is a specialized form of TerracoreHiveAction.
/// </summary>
public class OpenCrateAction : TerracoreHiveAction
{
    /// <summary>
    /// Initializes a new instance of the OpenCrateAction class with the specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the Open Crate Action on Terracore Hive.</param>
    /// <param name="crateType">The type of the crate to be opened.</param>
    public OpenCrateAction(Account account, CrateType crateType) : base(account, Constants.TransactionIds.OpenCrate, KeyType.Posting)
    {
        AddParameter("crate_type", crateType.ToString().ToLower());
        AddParameter("owner", account.Username);
    }
}