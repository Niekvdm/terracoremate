using Newtonsoft.Json;

namespace TerracoreMate.Hive.Operations;

public class CustomJson : IOperation
{
    public int opid => 18;
    public string[] required_auths;
    public string[] required_posting_auths;
    public string id;
    public object json;
}