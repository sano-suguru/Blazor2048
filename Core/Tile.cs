namespace Blazor2048.Core;

public class Tile(int value)
{
    public int Value { get; } = value;

    public static Tile[] MergeLine(Tile[] line, ref bool moved)
    {
        List<Tile> newLine = [];
        bool hasMerged = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i].Value == 0) continue;

            if (newLine.Count > 0 && newLine.Last().Value == line[i].Value && !hasMerged)
            {
                newLine[^1] = new Tile(newLine[^1].Value * 2);
                hasMerged = true;
                moved = true;
            }
            else
            {
                newLine.Add(new Tile(line[i].Value));
                hasMerged = false;
            }
        }

        while (newLine.Count < line.Length)
            newLine.Add(new Tile(0));

        return [.. newLine];
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Tile other) return false;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
