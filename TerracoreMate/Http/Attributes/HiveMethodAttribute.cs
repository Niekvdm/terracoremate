using Refit;
using TerracoreMate.Extensions;
using TerracoreMate.Hive.Enums;

namespace TerracoreMate.Http.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
public class HiveMethodAttribute<T> : HeadersAttribute where T : Enum
{
    public HiveMethodAttribute(HiveApiType apiType, T enumMethod) : base(
        $"method: {apiType.ToString().ToSnakeCase()}.{enumMethod.ToString().ToSnakeCase()}")
    {
    }
}

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
public class HiveMethodAttribute : HeadersAttribute
{
    public HiveMethodAttribute(HiveApiType apiType, string method) : base(
        $"method: {apiType.ToString().ToSnakeCase()}.{method}")
    {
    }
}