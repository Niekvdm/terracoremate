using System.Text.RegularExpressions;
using Cryptography.ECDSA;

namespace TerracoreMate.Hive.Encoding;

/// <summary>
/// Provides functionalities for encoding and decoding operations in HiveBase58 format.
/// </summary>
public class HiveBase58 : Base58
{
    /// <summary>
    /// Decodes private WIF (Wallet Import Format) from the provided data.
    /// </summary>
    /// <param name="data">The input data to decode.</param>
    public static byte[] DecodePrivateWif(string data)
    {
        if (data.All(Hexdigits.Contains))
            return Hex.HexToBytes(data);

        switch (data[0])
        {
            case '5':
            case '6':
                return Base58CheckDecode(data);
            case 'K':
            case 'L':
                return CutLastBytes(Base58CheckDecode(data), 1);
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>   
    /// Encodes private WIF (Wallet Import Format) from the provided byte array source.
    /// </summary>
    /// <param name="source">The byte array source to encode.</param>
    public static string EncodePrivateWif(byte[] source)
    {
        return Base58CheckEncode(0x80, source);
    }

    /// <summary>
    /// Validates the provided data for a private WIF (Wallet Import Format).
    /// </summary>
    /// <param name="data">The data to validate.</param>
    public static bool ValidatePrivateWif(string data)
    {
        try
        {
            _ = DecodePrivateWif(data);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Decodes public WIF (Wallet Import Format) using the provided publicKey and prefix.
    /// </summary>
    /// <param name="publicKey">The public key to decode.</param>
    /// <param name="prefix">The prefix to use in the decoding process.</param>
    public static byte[] DecodePublicWif(string publicKey, string prefix)
    {
        if (!publicKey.StartsWith(prefix))
            return Array.Empty<byte>();

        var buf = publicKey.Remove(0, prefix.Length);
        var s = Decode(buf);

        var checksum = BitConverter.ToInt32(s, s.Length - CheckSumSizeInBytes);
        var dec = RemoveCheckSum(s);
        var hash = Ripemd160Manager.GetHash(dec);
        var newChecksum = BitConverter.ToInt32(hash, 0);

        if (checksum != newChecksum)
            throw new ArithmeticException(nameof(checksum));

        return dec;
    }

    /// <summary>
    /// Encodes public WIF (Wallet Import Format) using the provided byte array publicKey and prefix.
    /// </summary>
    /// <param name="publicKey">The byte array public key to encode.</param>
    /// <param name="prefix">The prefix to use in the encoding process.</param>
    public static string EncodePublicWif(byte[] publicKey, string prefix)
    {
        var checksum = Ripemd160Manager.GetHash(publicKey);
        var s = AddLastBytes(publicKey, CheckSumSizeInBytes);
        Array.Copy(checksum, 0, s, s.Length - CheckSumSizeInBytes, CheckSumSizeInBytes);
        var pubdata = Encode(s);
        return prefix + pubdata;
    }

    /// <summary>
    /// Performs Base58Check decoding on the provided data.
    /// </summary>
    /// <param name="data">The data to decode.</param>
    public static byte[] Base58CheckDecode(string data)
    {
        var s = Decode(data);
        var dec = CutLastBytes(s, CheckSumSizeInBytes);

        var checksum = DoubleHash(dec);
        for (var i = 0; i < CheckSumSizeInBytes; i++)
        {
            if (checksum[i] != s[s.Length - CheckSumSizeInBytes + i])
                throw new ArithmeticException("Invalid WIF supplied");
        }

        return CutFirstBytes(dec, 1);
    }

    /// <summary>
    /// Performs Base58Check encoding on the provided version and data.
    /// </summary>
    /// <param name="version">The version to use in the encoding process.</param>
    /// <param name="data">The data to encode.</param>
    public static string Base58CheckEncode(byte version, byte[] data)
    {
        var s = AddFirstBytes(data, 1);
        s[0] = version;
        var checksum = DoubleHash(s);
        s = AddLastBytes(s, CheckSumSizeInBytes);
        Array.Copy(checksum, 0, s, s.Length - CheckSumSizeInBytes, CheckSumSizeInBytes);
        return Encode(s);
    }

    /// <summary>
    /// Generates a sub WIF (Wallet Import Format) from the supplied name, password, and role.
    /// </summary>
    /// <param name="name">The name to use in the generation process.</param>
    /// <param name="password">The password to use in the generation process.</param>
    /// <param name="role">The role to use in the generation process.</param>
    public static string GetSubWif(string name, string password, string role)
    {
        var seed = name + role + password;
        seed = Regex.Replace(seed, @"\s+", " ");
        var brainKey = System.Text.Encoding.ASCII.GetBytes(seed);
        var hashSha256 = Sha256Manager.GetHash(brainKey);
        return EncodePrivateWif(hashSha256);
    }
}