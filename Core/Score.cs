namespace Blazor2048.Core;

public readonly record struct Score(int Value)
{
    public static Score Zero => new(0);

    public static Score operator +(Score a, Score b) => new(a.Value + b.Value);
    public static Score operator -(Score a, Score b) => new(a.Value - b.Value);

    public override string ToString() => Value.ToString("N0");
}
