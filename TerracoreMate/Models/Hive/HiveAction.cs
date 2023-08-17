using Newtonsoft.Json;
using TerracoreMate.Hive;
using TerracoreMate.Hive.Operations;

namespace TerracoreMate.Models.Hive;

/// <summary>
/// Represents an action to be performed on a Hive node. It wraps the parameters and methods needed 
/// for transaction creation, signature and serialization into a single class.
/// </summary>
public class HiveAction : Dictionary<string, object>
{
    private readonly string _transactionId;
    private readonly KeyType _keyType;
    private bool _watermark = true;
    private readonly Account _account;
    private Dictionary<string, object> Parameters { get; } = new();

    protected string TransactionId => _transactionId;

    /// <summary>
    /// Initializes a new instance of the HiveAction class with the specified JSON-RPC method.
    /// </summary>
    /// <param name="method">The name of the method to be invoked on the Hive node.</param>
    public HiveAction(string method)
    {
        Add("id", 1);
        Add("jsonrpc", "2.0");
        Add("method", method);
    }

    /// <summary>
    /// Initializes a new instance of the HiveAction class with specified parameters.
    /// </summary>
    /// <param name="account">The account associated with the transaction.</param>
    /// <param name="transactionId">The unique identifier of the transaction.</param>
    /// <param name="keyType">The type of access key.</param>
    /// <param name="method">The name of the method to be invoked on the Hive node.</param>
    public HiveAction(Account account, string transactionId, KeyType keyType, string method)
    {
        this._transactionId = transactionId;
        _keyType = keyType;
        _account = account;

        Add("id", 1);
        Add("jsonrpc", "2.0");
        Add("method", $"condenser_api.{method}");
    }

    /// <summary>
    /// Disables the watermark setting in the transaction.
    /// </summary>
    public void DisableWatermark()
    {
        _watermark = false;
    }

    /// <summary>
    /// Adds a new parameter to hive action.
    /// </summary>
    /// <param name="key">Key of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    public void AddParameter(string key, object value)
    {
        Parameters.TryAdd(key, value);
    }

    /// <summary>
    /// Creates a signed transaction.
    /// </summary>
    /// <param name="hiveSigner">The object that will sign the transaction.</param>
    /// <returns>A Task representing the asynchronous operation, with a built and signed transaction.</returns>
    public async Task<Dictionary<string, object>> ToSignedTransaction(HiveSigner hiveSigner)
    {
        if (_watermark)
            AddParameter("app", "terracoremate");

        var customJsonOperation = new CustomJson
        {
            id = _transactionId,
            required_auths = _keyType == KeyType.Active ? new[] { _account.Username } : Array.Empty<string>(),
            required_posting_auths = _keyType == KeyType.Posting ? new[] { _account.Username } : Array.Empty<string>(),
            json = JsonConvert.SerializeObject(Parameters)
        };

        var transaction = await hiveSigner.BundleTransactionOperations(
            new object[] { customJsonOperation },
            new[] { _account.GetKey(_keyType) }
        );

        Add("params", new object[] { transaction.tx });

        return this;
    }

    /// <summary>
    /// Converts the HiveAction object into a dictionary, for use as the body of a request.
    /// </summary>
    /// <returns>The HiveAction object, represented as a Dictionary.</returns>
    public Dictionary<string, object> ToBody()
    {
        return this;
    }
}