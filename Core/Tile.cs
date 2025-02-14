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

        // まず非空のタイルを圧縮して移動
        foreach (var tile in line.Where(t => !t.IsEmpty))
        {
            newLine.Add(new Tile(tile.Value));
        }

        // 元の配列と長さが違えば移動が発生している
        if (newLine.Count != line.Count(t => !t.IsEmpty))
        {
            moved = true;
        }

        // マージ処理
        for (int i = 0; i < newLine.Count - 1; i++)
        {
            if (newLine[i].Value == newLine[i + 1].Value)
            {
                newLine[i] = new Tile(newLine[i].Value * 2);
                newLine.RemoveAt(i + 1);
                moved = true;
            }
        }

        // 残りのスペースを0で埋める
        while (newLine.Count < line.Length)
        {
            newLine.Add(Tile.Empty);
        }

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
