using Microsoft.Extensions.Logging;

namespace Blazor2048.Core.Commands;

/// <summary>
/// 移動コマンドファクトリーの実装
/// 各方向の移動コマンドを作成・管理する
/// </summary>
public class MoveCommandFactory : IMoveCommandFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly Dictionary<Direction, Func<IMoveCommand>> _commandFactories;

    public MoveCommandFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        
        _commandFactories = new Dictionary<Direction, Func<IMoveCommand>>
        {
            { Direction.Left, () => new MoveLeftCommand(_loggerFactory.CreateLogger<MoveLeftCommand>()) },
            { Direction.Right, () => new MoveRightCommand(_loggerFactory.CreateLogger<MoveRightCommand>()) },
            { Direction.Up, () => new MoveUpCommand(_loggerFactory.CreateLogger<MoveUpCommand>()) },
            { Direction.Down, () => new MoveDownCommand(_loggerFactory.CreateLogger<MoveDownCommand>()) }
        };
    }

    public IMoveCommand CreateCommand(Direction direction)
    {
        if (!_commandFactories.TryGetValue(direction, out var factory))
        {
            throw new ArgumentException($"Unsupported move direction: {direction}", nameof(direction));
        }

        return factory();
    }

    public IEnumerable<IMoveCommand> GetAllCommands()
    {
        return _commandFactories.Values.Select(factory => factory());
    }
}

/// <summary>
/// 右方向への移動コマンド
/// </summary>
public class MoveRightCommand : MoveCommandBase
{
    public override Direction Direction => Direction.Right;

    public MoveRightCommand(ILogger<MoveRightCommand> logger) : base(logger)
    {
    }

    protected override Result<MoveResult> ExecuteMove(IGameBoard board, bool isPreview = false)
    {
        if (board is not Board concreteBoard)
        {
            return Result<MoveResult>.Failure("Board type not supported for move command");
        }

        var totalTilesMoved = 0;
        var totalTilesMerged = 0;
        var totalScoreGained = 0;
        var allMergeEvents = new List<TileMergedEventArgs>();
        var hasMoved = false;

        try
        {
            // 各行を処理（右方向なので逆順）
            for (var row = 0; row < GameConstants.BoardSize; row++)
            {
                var line = GetLine(board, row);
                var moveResult = MergeLine(line);

                if (moveResult.HasMoved)
                {
                    hasMoved = true;
                    totalTilesMoved += moveResult.TilesMoved;
                    totalTilesMerged += moveResult.TilesMerged;
                    totalScoreGained += moveResult.ScoreGained.Value;

                    // マージイベントの位置を調整（右方向なので列インデックスを逆転）
                    var adjustedMergeEvents = moveResult.MergeEvents
                        .Select(e => new TileMergedEventArgs(
                            new Position(row, GameConstants.BoardSize - 1 - e.Position.Column),
                            e.OldValue,
                            e.NewValue))
                        .ToList();
                    
                    allMergeEvents.AddRange(adjustedMergeEvents);

                    // 結果をボードに設定
                    SetLine(board, row, moveResult.ResultLine);
                }
            }

            // プレビューでない場合は新しいタイルを追加
            if (!isPreview && hasMoved)
            {
                concreteBoard.AddNewTile();
            }

            var result = hasMoved
                ? MoveResult.Success(totalTilesMoved, totalTilesMerged, new Score(totalScoreGained), allMergeEvents)
                : MoveResult.NoMove();

            Logger.LogDebug("Move right completed: {Result}", result);
            return Result<MoveResult>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing right move");
            return Result<MoveResult>.Failure($"Failed to execute right move: {ex.Message}");
        }
    }

    protected override ReadOnlySpan<Tile> GetLine(IGameBoard board, int lineIndex)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        var line = new Tile[GameConstants.BoardSize];
        // 右方向なので逆順で取得
        for (var col = 0; col < GameConstants.BoardSize; col++)
        {
            line[col] = concreteBoard.Tiles[lineIndex, GameConstants.BoardSize - 1 - col];
        }
        return line.AsSpan();
    }

    protected override void SetLine(IGameBoard board, int lineIndex, ReadOnlySpan<Tile> line)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        // 右方向なので逆順で設定
        for (var col = 0; col < GameConstants.BoardSize; col++)
        {
            concreteBoard.Tiles[lineIndex, GameConstants.BoardSize - 1 - col] = line[col];
        }
    }

    protected override IGameBoard CreateBoardCopy(IGameBoard original)
    {
        if (original is not Board originalBoard)
            throw new ArgumentException("Board type not supported", nameof(original));

        return originalBoard;
    }
}

/// <summary>
/// 上方向への移動コマンド
/// </summary>
public class MoveUpCommand : MoveCommandBase
{
    public override Direction Direction => Direction.Up;

    public MoveUpCommand(ILogger<MoveUpCommand> logger) : base(logger)
    {
    }

    protected override Result<MoveResult> ExecuteMove(IGameBoard board, bool isPreview = false)
    {
        if (board is not Board concreteBoard)
        {
            return Result<MoveResult>.Failure("Board type not supported for move command");
        }

        var totalTilesMoved = 0;
        var totalTilesMerged = 0;
        var totalScoreGained = 0;
        var allMergeEvents = new List<TileMergedEventArgs>();
        var hasMoved = false;

        try
        {
            // 各列を処理
            for (var col = 0; col < GameConstants.BoardSize; col++)
            {
                var line = GetLine(board, col);
                var moveResult = MergeLine(line);

                if (moveResult.HasMoved)
                {
                    hasMoved = true;
                    totalTilesMoved += moveResult.TilesMoved;
                    totalTilesMerged += moveResult.TilesMerged;
                    totalScoreGained += moveResult.ScoreGained.Value;

                    var adjustedMergeEvents = moveResult.MergeEvents
                        .Select(e => new TileMergedEventArgs(
                            new Position(e.Position.Column, col), // 行と列を入れ替え
                            e.OldValue,
                            e.NewValue))
                        .ToList();
                    
                    allMergeEvents.AddRange(adjustedMergeEvents);

                    SetLine(board, col, moveResult.ResultLine);
                }
            }

            if (!isPreview && hasMoved)
            {
                concreteBoard.AddNewTile();
            }

            var result = hasMoved
                ? MoveResult.Success(totalTilesMoved, totalTilesMerged, new Score(totalScoreGained), allMergeEvents)
                : MoveResult.NoMove();

            Logger.LogDebug("Move up completed: {Result}", result);
            return Result<MoveResult>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing up move");
            return Result<MoveResult>.Failure($"Failed to execute up move: {ex.Message}");
        }
    }

    protected override ReadOnlySpan<Tile> GetLine(IGameBoard board, int lineIndex)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        var line = new Tile[GameConstants.BoardSize];
        for (var row = 0; row < GameConstants.BoardSize; row++)
        {
            line[row] = concreteBoard.Tiles[row, lineIndex];
        }
        return line.AsSpan();
    }

    protected override void SetLine(IGameBoard board, int lineIndex, ReadOnlySpan<Tile> line)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        for (var row = 0; row < GameConstants.BoardSize; row++)
        {
            concreteBoard.Tiles[row, lineIndex] = line[row];
        }
    }

    protected override IGameBoard CreateBoardCopy(IGameBoard original)
    {
        if (original is not Board originalBoard)
            throw new ArgumentException("Board type not supported", nameof(original));

        return originalBoard;
    }
}

/// <summary>
/// 下方向への移動コマンド
/// </summary>
public class MoveDownCommand : MoveCommandBase
{
    public override Direction Direction => Direction.Down;

    public MoveDownCommand(ILogger<MoveDownCommand> logger) : base(logger)
    {
    }

    protected override Result<MoveResult> ExecuteMove(IGameBoard board, bool isPreview = false)
    {
        if (board is not Board concreteBoard)
        {
            return Result<MoveResult>.Failure("Board type not supported for move command");
        }

        var totalTilesMoved = 0;
        var totalTilesMerged = 0;
        var totalScoreGained = 0;
        var allMergeEvents = new List<TileMergedEventArgs>();
        var hasMoved = false;

        try
        {
            // 各列を処理（下方向なので逆順）
            for (var col = 0; col < GameConstants.BoardSize; col++)
            {
                var line = GetLine(board, col);
                var moveResult = MergeLine(line);

                if (moveResult.HasMoved)
                {
                    hasMoved = true;
                    totalTilesMoved += moveResult.TilesMoved;
                    totalTilesMerged += moveResult.TilesMerged;
                    totalScoreGained += moveResult.ScoreGained.Value;

                    var adjustedMergeEvents = moveResult.MergeEvents
                        .Select(e => new TileMergedEventArgs(
                            new Position(GameConstants.BoardSize - 1 - e.Position.Column, col),
                            e.OldValue,
                            e.NewValue))
                        .ToList();
                    
                    allMergeEvents.AddRange(adjustedMergeEvents);

                    SetLine(board, col, moveResult.ResultLine);
                }
            }

            if (!isPreview && hasMoved)
            {
                concreteBoard.AddNewTile();
            }

            var result = hasMoved
                ? MoveResult.Success(totalTilesMoved, totalTilesMerged, new Score(totalScoreGained), allMergeEvents)
                : MoveResult.NoMove();

            Logger.LogDebug("Move down completed: {Result}", result);
            return Result<MoveResult>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing down move");
            return Result<MoveResult>.Failure($"Failed to execute down move: {ex.Message}");
        }
    }

    protected override ReadOnlySpan<Tile> GetLine(IGameBoard board, int lineIndex)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        var line = new Tile[GameConstants.BoardSize];
        // 下方向なので逆順で取得
        for (var row = 0; row < GameConstants.BoardSize; row++)
        {
            line[row] = concreteBoard.Tiles[GameConstants.BoardSize - 1 - row, lineIndex];
        }
        return line.AsSpan();
    }

    protected override void SetLine(IGameBoard board, int lineIndex, ReadOnlySpan<Tile> line)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        // 下方向なので逆順で設定
        for (var row = 0; row < GameConstants.BoardSize; row++)
        {
            concreteBoard.Tiles[GameConstants.BoardSize - 1 - row, lineIndex] = line[row];
        }
    }

    protected override IGameBoard CreateBoardCopy(IGameBoard original)
    {
        if (original is not Board originalBoard)
            throw new ArgumentException("Board type not supported", nameof(original));

        return originalBoard;
    }
}
