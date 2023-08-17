using Refit;

namespace TerracoreMate.Http.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class HiveAttribute : PropertyAttribute
{
    public HiveAttribute() : base("Hive")
    {
    }
}

