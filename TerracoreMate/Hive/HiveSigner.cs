using Cryptography.ECDSA;
using TerracoreMate.Extensions;
using TerracoreMate.Hive.Encoding;
using TerracoreMate.Hive.Models;
using TerracoreMate.Http.Services;

namespace TerracoreMate.Hive;

public class HiveSigner
 {
     private readonly IHiveGlobalService _hiveGlobalService;
     private readonly ILogger _logger;
     private const string ChainId = "beeab0de00000000000000000000000000000000000000000000000000000000";
 
     /// <summary>
     /// Constructor for the HiveSigner class.
     /// </summary>
     /// <param name="hiveGlobalService">Injected service for handling Hive global properties.</param>
     /// <param name="logger">Injected logger service for logging events and exceptions.</param>
     public HiveSigner(IHiveGlobalService hiveGlobalService, ILogger logger)
     {
         _hiveGlobalService = hiveGlobalService;
         _logger = logger;
     }
 
     /// <summary>
     /// Creates a transaction, signs it with provided keys, and bundles the operations of the transaction into one entity.
     /// </summary>
     /// <param name="operations">Array of operation objects to be included in the transaction.</param>
     /// <param name="keys">The keys used for signing the transaction.</param>
     /// <returns>A Task that represents the asynchronous operation. The task result contains the final transaction object that's signed and ready to be pushed on the blockchain.</returns>
     public async Task<Transaction?> BundleTransactionOperations(object[] operations, IEnumerable<string> keys)
     {
         var transaction = await CreateTransaction(operations, keys);
 
         if (transaction == null)
         {
             return null;
         }
 
         for (var i = 0; i < transaction.tx.operations.Length; i++)
         {
             var op = transaction.tx.operations[i];
             transaction.tx.operations[i] = new Op { name = op.GetType().Name.ToSnakeCase(), payload = op };
         }
 
         return transaction;
     }
 
     /// <summary>
     /// Internal method to create transaction with given operations
     /// </summary>
     /// <param name="operations">Array of operation objects.</param>
     /// <param name="keys">Keys for signing the transaction.</param>
     /// <param name="errorCount">Number if errors occurred during the creation. Default is 0.</param>
     /// <returns>A Transaction signed and ready for bundling.</returns>
     private async Task<Transaction?> CreateTransaction(object[] operations, IEnumerable<string> keys, int errorCount = 0)
     {
         try
         {
             var oDGP = await _hiveGlobalService.RetrieveDynamicGlobalProperties();
 
             var transaction = new TransactionBody
             {
                 ref_block_num = Convert.ToUInt16((uint) oDGP["result"]["head_block_number"] & 0xFFFF),
                 ref_block_prefix = BitConverter.ToUInt32(Hex.HexToBytes(oDGP["result"]["head_block_id"].ToString()), 4),
                 expiration = Convert.ToDateTime(oDGP["result"]["time"]).AddMinutes(5),
                 operations = operations
             };
 
             return SignTransaction(transaction, keys);
         }
         catch (Exception ex)
         {
             if (errorCount > 5 || !ex.Message.Contains("Internal Error"))
             {
                 _logger.Error(ex, "Exception occurred during signing blockchain transaction [retry in 10s]: {Message}",
                     ex.Message);
                 return null;
             }
 
             await Task.Delay(10 * 1000);
             return await CreateTransaction(operations, keys, errorCount + 1);
         }
     }
 
     /// <summary>
     /// Sign a given transaction with a list of keys
     /// </summary>
     /// <param name="body">The transaction body to be signed</param>
     /// <param name="keys">The list of keys for signing the transaction</param>
     /// <returns>A signed Transaction.</returns>
     private Transaction? SignTransaction(TransactionBody body, IEnumerable<string> keys)
     {
         try
         {
             var serializer = new Serializer();
 
             var msg = serializer.Serialize(body);

             using var memoryStream = new MemoryStream();
             
             var chainIdBytes = Hex.HexToBytes(ChainId);
             memoryStream.Write(chainIdBytes, 0, chainIdBytes.Length);
             memoryStream.Write(msg, 0, msg.Length);
 
             var digest = Sha256Manager.GetHash(memoryStream.ToArray());
 
             foreach (var key in keys)
             {
                 var signatures = body.signatures.Append(Hex.ToString(Secp256K1Manager.SignCompressedCompact(digest, HiveBase58.DecodePrivateWif(key))));
 
                 body.signatures = signatures.ToArray();
             }
 
             return new Transaction { tx = body, txid = Hex.ToString(Sha256Manager.GetHash(msg)).Substring(0, 40) };
         }
         catch (Exception ex)
         {
             _logger.Error(ex, "Exception occurred during signing blockchain transaction: {Message}", ex.Message);
             return null;
         }
     }
 }