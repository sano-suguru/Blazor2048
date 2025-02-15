namespace Blazor2048.Core;

public record HighScore
{
    public Score Value { get; init; }
    public DateTime AchievedAt { get; init; }

    public static HighScore Create(Score score) => new()
    {
        Value = score,
        AchievedAt = DateTime.UtcNow
    };
}
