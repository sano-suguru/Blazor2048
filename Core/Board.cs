namespace Blazor2048.Core;

public class Board
{
    public const int Size = 4;
    public Tile[,] Tiles { get; private set; }
    private static readonly Random random = new();

    private static readonly Dictionary<Direction, Func<Board, int, Tile[]>> GetLineMap = new()
    {
        { Direction.Left, (board, i) => Enumerable.Range(0, Size).Select(j => board.Tiles[i, j]).ToArray() },
        { Direction.Right, (board, i) => Enumerable.Range(0, Size).Select(j => board.Tiles[i, Size - 1 - j]).ToArray() },
        { Direction.Up, (board, i) => Enumerable.Range(0, Size).Select(j => board.Tiles[j, i]).ToArray() },
        { Direction.Down, (board, i) => Enumerable.Range(0, Size).Select(j => board.Tiles[Size - 1 - j, i]).ToArray() },
    };

    private static readonly Dictionary<Direction, Action<Board, int, Tile[]>> SetLineMap = new()
    {
        { Direction.Left, (board, i, line) => { for (int j = 0; j < Size; j++) board.Tiles[i, j] = line[j]; } },
        { Direction.Right, (board, i, line) => { for (int j = 0; j < Size; j++) board.Tiles[i, Size - 1 - j] = line[j]; } },
        { Direction.Up, (board, i, line) => { for (int j = 0; j < Size; j++) board.Tiles[j, i] = line[j]; } },
        { Direction.Down, (board, i, line) => { for (int j = 0; j < Size; j++) board.Tiles[Size - 1 - j, i] = line[j]; } },
    };

    public Board()
    {
        Tiles = new Tile[Size, Size];
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                Tiles[i, j] = new Tile(0);

        AddNewTile();
        AddNewTile();
    }

    public bool MoveTiles(Direction direction)
    {
        bool moved = false;
        Tile[,] originalTiles = (Tile[,])Tiles.Clone();

        for (int i = 0; i < Size; i++)
        {
            Tile[] line = GetLineMap[direction](this, i);
            Tile[] mergedLine = Tile.MergeLine(line, ref moved);
            SetLineMap[direction](this, i, mergedLine);
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
                if (Tiles[i, j].Value == 0) emptyCells.Add((i, j));

        if (emptyCells.Count > 0)
        {
            var (x, y) = emptyCells[random.Next(emptyCells.Count)];
            Tiles[x, y] = new Tile(random.Next(100) < 90 ? 2 : 4);
        }
    }

    private static bool AreBoardsEqual(Tile[,] board1, Tile[,] board2)
    {
        return board1.Cast<Tile>().SequenceEqual(board2.Cast<Tile>());
    }
}
