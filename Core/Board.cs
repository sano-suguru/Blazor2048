namespace Blazor2048.Core;

public class Board
{
    public const int Size = 4;
    public int[,] Tiles { get; private set; }
    public int Score { get; private set; }
    private static readonly Random random = new();

    private static readonly Dictionary<Direction, Func<int[,], int, int[]>> GetLineMap = new()
    {
        { Direction.Left, (tiles, i) => Enumerable.Range(0, Size).Select(j => tiles[i, j]).ToArray() },
        { Direction.Right, (tiles, i) => Enumerable.Range(0, Size).Select(j => tiles[i, Size - 1 - j]).ToArray() },
        { Direction.Up, (tiles, i) => Enumerable.Range(0, Size).Select(j => tiles[j, i]).ToArray() },
        { Direction.Down, (tiles, i) => Enumerable.Range(0, Size).Select(j => tiles[Size - 1 - j, i]).ToArray() },
    };

    private static readonly Dictionary<Direction, Action<int[,], int, int[]>> SetLineMap = new()
    {
        { Direction.Left, (tiles, i, line) => { for (int j = 0; j < Size; j++) tiles[i, j] = line[j]; } },
        { Direction.Right, (tiles, i, line) => { for (int j = 0; j < Size; j++) tiles[i, Size - 1 - j] = line[j]; } },
        { Direction.Up, (tiles, i, line) => { for (int j = 0; j < Size; j++) tiles[j, i] = line[j]; } },
        { Direction.Down, (tiles, i, line) => { for (int j = 0; j < Size; j++) tiles[Size - 1 - j, i] = line[j]; } },
    };

    public Board()
    {
        Tiles = new int[Size, Size];
        Score = 0;
        AddNewTile();
        AddNewTile();
    }

    public bool MoveTiles(Direction direction)
    {
        bool moved = false;
        int[,] originalTiles = (int[,])Tiles.Clone();

        for (int i = 0; i < Size; i++)
        {
            int[] line = GetLineMap[direction](Tiles, i);
            int[] mergedLine = Tile.MergeLine(line, ref moved);
            SetLineMap[direction](Tiles, i, mergedLine);
        }

        if (!AreBoardsEqual(originalTiles, Tiles))
        {
            AddNewTile();
            moved = true;
        }

        return moved;
    }

    public void AddNewTile()
    {
        List<(int, int)> emptyCells = new();

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (Tiles[i, j] == 0) emptyCells.Add((i, j));

        if (emptyCells.Count > 0)
        {
            var (x, y) = emptyCells[random.Next(emptyCells.Count)];
            Tiles[x, y] = random.Next(100) < 90 ? 2 : 4;
        }
    }

    private static bool AreBoardsEqual(int[,] board1, int[,] board2)
    {
        return board1.Cast<int>().SequenceEqual(board2.Cast<int>());
    }
}
