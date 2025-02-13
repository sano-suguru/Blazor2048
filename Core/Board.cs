namespace Blazor2048.Core;

public class Board : IGameBoard
{
    public Tile[,] Tiles { get; private set; }
    private readonly IRandomGenerator _random;
    private readonly ILogger<Board> _logger;

    private static readonly Dictionary<Direction, Func<Board, int, Tile[]>> GetLineMap = new()
    {
        { Direction.Left, (board, i) => Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[i, j]).ToArray() },
        { Direction.Right, (board, i) => Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[i, GameConstants.BoardSize - 1 - j]).ToArray() },
        { Direction.Up, (board, i) => Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[j, i]).ToArray() },
        { Direction.Down, (board, i) => Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[GameConstants.BoardSize - 1 - j, i]).ToArray() },
    };

    private static readonly Dictionary<Direction, Action<Board, int, Tile[]>> SetLineMap = new()
    {
        { Direction.Left, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[i, j] = line[j]; } },
        { Direction.Right, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[i, GameConstants.BoardSize - 1 - j] = line[j]; } },
        { Direction.Up, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[j, i] = line[j]; } },
        { Direction.Down, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[GameConstants.BoardSize - 1 - j, i] = line[j]; } },
    };

    public Board(IRandomGenerator random, ILogger<Board> logger)
    {
        _random = random;
        _logger = logger;
        Tiles = new Tile[GameConstants.BoardSize, GameConstants.BoardSize];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < GameConstants.BoardSize; i++)
            for (int j = 0; j < GameConstants.BoardSize; j++)
                Tiles[i, j] = new Tile(0);

        for (int i = 0; i < GameConstants.InitialTileCount; i++)
        {
            AddNewTile();
        }
    }

    public bool MoveTiles(Direction direction)
    {
        bool moved = false;
        Tile[,] originalTiles = (Tile[,])Tiles.Clone();

        try
        {
            for (int i = 0; i < GameConstants.BoardSize; i++)
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tiles in direction {Direction}", direction);
            throw new GameException($"Failed to move tiles: {ex.Message}");
        }

        return moved;
    }

    public void AddNewTile()
    {
        var emptyPositions = GetEmptyPositions();

        if (!emptyPositions.Any())
        {
            _logger.LogWarning("Attempted to add new tile but no empty positions available");
            throw new GameException("No empty cells available");
        }

        var position = emptyPositions[_random.Next(emptyPositions.Count)];
        var newValue = _random.Next(100) < GameConstants.NewTileProbability2 ? 2 : 4;
        Tiles[position.Row, position.Column] = new Tile(newValue);

        _logger.LogInformation("Added new tile with value {Value} at position {Position}", newValue, position);
    }

    public bool IsGameOver()
    {
        // Check for empty cells
        if (GetEmptyPositions().Count != 0) return false;

        // Check for possible merges
        for (int i = 0; i < GameConstants.BoardSize; i++)
        {
            for (int j = 0; j < GameConstants.BoardSize; j++)
            {
                var currentValue = Tiles[i, j].Value;

                // Check right neighbor
                if (j < GameConstants.BoardSize - 1 && Tiles[i, j + 1].Value == currentValue)
                    return false;

                // Check bottom neighbor
                if (i < GameConstants.BoardSize - 1 && Tiles[i + 1, j].Value == currentValue)
                    return false;
            }
        }

        return true;
    }

    private List<Position> GetEmptyPositions() =>
        [.. Enumerable.Range(0, GameConstants.BoardSize)
            .SelectMany(i => Enumerable.Range(0, GameConstants.BoardSize)
                .Where(j => Tiles[i, j].Value == 0)
                .Select(j => new Position(i, j)))];

    private static bool AreBoardsEqual(Tile[,] board1, Tile[,] board2) =>
        board1.Cast<Tile>().SequenceEqual(board2.Cast<Tile>());
}
