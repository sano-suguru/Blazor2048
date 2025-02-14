namespace Blazor2048.Core;

public class Tile
{
    public int Value { get; }
    public bool IsMerged { get; private set; }

    public Tile(int value, bool isMerged = false)
    {
        Value = value;
        IsMerged = isMerged;
    }

    public static Tile Empty => new(0);

    public bool IsEmpty => Value == 0;

    public bool CanMergeWith(Tile other) =>
        !IsEmpty && !IsMerged && !other.IsMerged && Value == other.Value;

    public Tile MergeWith(Tile other)
    {
        if (!CanMergeWith(other))
            throw new GameException("Cannot merge incompatible tiles");

        return new Tile(Value * 2, true);
    }

    public static Tile[] MergeLine(Tile[] line, ref bool moved)
    {
        var newLine = new List<Tile>();
        var hasMerged = false;

        foreach (var tile in line.Where(t => !t.IsEmpty))
        {
            if (newLine.Count > 0 && !hasMerged && newLine[^1].CanMergeWith(tile))
            {
                newLine[^1] = newLine[^1].MergeWith(tile);
                hasMerged = true;
                moved = true;
            }
            else
            {
                newLine.Add(new Tile(tile.Value));
                hasMerged = false;
            }
        }

        while (newLine.Count < line.Length)
            newLine.Add(Tile.Empty);

        return [.. newLine];
    }

    public override bool Equals(object? obj) =>
        obj is Tile other && Value == other.Value && IsMerged == other.IsMerged;

    public override int GetHashCode() =>
        HashCode.Combine(Value, IsMerged);

    public static bool operator ==(Tile? left, Tile? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Tile? left, Tile? right) =>
        !(left == right);
}
