using Refit;

namespace TerracoreMate.Http.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HivePostAttribute : HttpMethodAttribute
{
    public HivePostAttribute(string? path = null) : base(path ?? "")
    {
    }
    
    public override HttpMethod Method => HttpMethod.Post;
}