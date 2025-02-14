// Core/Board.cs
namespace Blazor2048.Core;

public class Board(IRandomGenerator random, ILogger<Board> logger) : IGameBoard
{
    private readonly IRandomGenerator _random = random;
    private readonly ILogger<Board> _logger = logger;

    public Tile[,] Tiles { get; private set; } = new Tile[GameConstants.BoardSize, GameConstants.BoardSize];
    public event EventHandler<TileMergedEventArgs>? TileMerged;

    private static readonly Dictionary<Direction, Func<Board, int, Tile[]>> GetLineMap = new()
    {
        { Direction.Left, (board, i) => [.. Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[i, j])] },
        { Direction.Right, (board, i) => [.. Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[i, GameConstants.BoardSize - 1 - j])] },
        { Direction.Up, (board, i) => [.. Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[j, i])] },
        { Direction.Down, (board, i) => [.. Enumerable.Range(0, GameConstants.BoardSize).Select(j => board.Tiles[GameConstants.BoardSize - 1 - j, i])] }
    };

    private static readonly Dictionary<Direction, Action<Board, int, Tile[]>> SetLineMap = new()
    {
        { Direction.Left, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[i, j] = line[j]; } },
        { Direction.Right, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[i, GameConstants.BoardSize - 1 - j] = line[j]; } },
        { Direction.Up, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[j, i] = line[j]; } },
        { Direction.Down, (board, i, line) => { for (int j = 0; j < GameConstants.BoardSize; j++) board.Tiles[GameConstants.BoardSize - 1 - j, i] = line[j]; } }
    };

    public Board(IRandomGenerator random, ILogger<Board> logger, int[,] initialState) : this(random, logger)
    {
        if (initialState.GetLength(0) != GameConstants.BoardSize || initialState.GetLength(1) != GameConstants.BoardSize)
            throw new GameException("Invalid initial state dimensions");

        for (int i = 0; i < GameConstants.BoardSize; i++)
            for (int j = 0; j < GameConstants.BoardSize; j++)
                Tiles[i, j] = new Tile(initialState[i, j]);
    }

    public Board(IRandomGenerator random, ILogger<Board> logger, Board other) : this(random, logger)
    {
        Tiles = (Tile[,])other.Tiles.Clone();
    }

    private void Initialize()
    {
        for (int i = 0; i < GameConstants.BoardSize; i++)
            for (int j = 0; j < GameConstants.BoardSize; j++)
                Tiles[i, j] = Tile.Empty;

        for (int i = 0; i < GameConstants.InitialTileCount; i++)
            AddNewTile();
    }

    public bool MoveTiles(Direction direction)
    {
        bool moved = false;
        var originalTiles = (Tile[,])Tiles.Clone();

        try
        {
            for (int i = 0; i < GameConstants.BoardSize; i++)
            {
                Tile[] line = GetLineMap[direction](this, i);
                Tile[] mergedLine = Tile.MergeLine(line, ref moved);
                SetLineMap[direction](this, i, mergedLine);
            }

            if (moved)
            {
                AddNewTile();
                CheckAndRaiseMergeEvents(originalTiles);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving tiles in direction {Direction}", direction);
            throw new GameException($"Failed to move tiles: {ex.Message}", ex);
        }

        return moved;
    }

    public bool CanMove(Direction direction)
    {
        var tempBoard = new Board(_random, _logger, this);
        return tempBoard.MoveTiles(direction);
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
        if (GetEmptyPositions().Any()) return false;

        // Check for possible merges in all directions
        return !Enum.GetValues<Direction>().Any(CanMove);
    }

    private List<Position> GetEmptyPositions() =>
        [.. Enumerable.Range(0, GameConstants.BoardSize)
            .SelectMany(row =>
                Enumerable.Range(0, GameConstants.BoardSize)
                    .Where(col => Tiles[row, col].IsEmpty)
                    .Select(col => new Position(row, col)))];

    private void CheckAndRaiseMergeEvents(Tile[,] originalTiles)
    {
        for (int i = 0; i < GameConstants.BoardSize; i++)
        {
            for (int j = 0; j < GameConstants.BoardSize; j++)
            {
                var originalValue = originalTiles[i, j].Value;
                var newValue = Tiles[i, j].Value;

                if (newValue > originalValue && !Tiles[i, j].IsEmpty)
                {
                    OnTileMerged(new TileMergedEventArgs(
                        new Position(i, j),
                        originalValue,
                        newValue
                    ));
                }
            }
        }
    }

    protected virtual void OnTileMerged(TileMergedEventArgs e)
    {
        _logger.LogDebug("Tile merged at position {Position}: {OldValue} -> {NewValue}",
            e.Position, e.OldValue, e.NewValue);
        TileMerged?.Invoke(this, e);
    }
}
