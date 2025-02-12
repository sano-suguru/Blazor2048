namespace Blazor2048.Core;

public static class Tile
{
    public static int[] MergeLine(int[] line, ref bool moved)
    {
        List<int> newLine = new();
        bool hasMerged = false;
        int lineScore = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == 0) continue;

            if (newLine.Count > 0 && newLine.Last() == line[i] && !hasMerged)
            {
                newLine[^1] *= 2;
                lineScore += newLine[^1];
                hasMerged = true;
                moved = true;
            }
            else
            {
                newLine.Add(line[i]);
                hasMerged = false;
            }
        }

        while (newLine.Count < line.Length)
            newLine.Add(0);

        return newLine.ToArray();
    }
}
