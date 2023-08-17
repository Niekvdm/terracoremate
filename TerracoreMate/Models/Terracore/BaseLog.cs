namespace TerracoreMate.Models.Terracore;

public abstract class BaseLog
{
    public virtual long Timestamp { get; set; }

    public DateTimeOffset OccuredAt => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);
    
    public virtual bool HasOccuredWithinSeconds(int seconds)
    {
        var occuredAtSeconds = Math.Abs(OccuredAt.Subtract(DateTimeOffset.UtcNow).TotalSeconds);

        return seconds >= occuredAtSeconds;
    }
}