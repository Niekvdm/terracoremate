using System.Globalization;
using Newtonsoft.Json;
using TerracoreMate.Hive.Converters;

namespace TerracoreMate.Hive.Models;

[JsonConverter(typeof(AssetJsonConverter))] public class Asset
{
    public decimal amount;
    public string symbol;

    private static string CheckSymbol(string strSymbol)
    {
        string[] aSymbols = { "HIVE", "HBD", "TESTS", "TBD", "VESTS" };

        if (string.IsNullOrEmpty(strSymbol) || !aSymbols.Contains(strSymbol))
        {
            throw new Exception(string.Format("Invalid asset symbol: {0}", strSymbol));
        }
        return strSymbol;
    }

    public Asset(decimal strAmount, string strSymbol)  
    {
        amount = strAmount;
        symbol = CheckSymbol(strSymbol);
    }

    public Asset(string str)
    {
        var astr = str.Split(' ');

        if (astr.Length < 2)
        {
            throw new Exception("Invalid asset string");
        }
        if (string.IsNullOrEmpty(astr[0]))
        {
            throw new Exception("Invalid asset amount (null or empty)");
        }
        amount = decimal.Parse(astr[0], CultureInfo.InvariantCulture);
        symbol = CheckSymbol(astr[1]);
    }

    public int GetPrecision()
    {
        string[] symbols = { "HIVE", "HBD", "TESTS", "TBD" };
        return symbols.Contains(symbol) ? 3 : 6;
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "{0:0." + new string('0', GetPrecision()) + "} {1}", amount, symbol);
    }
}