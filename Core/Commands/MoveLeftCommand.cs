using Microsoft.Extensions.Logging;

namespace Blazor2048.Core.Commands;

/// <summary>
/// 左方向への移動コマンド
/// </summary>
public class MoveLeftCommand : MoveCommandBase
{
    public override Direction Direction => Direction.Left;

    public MoveLeftCommand(ILogger<MoveLeftCommand> logger) : base(logger)
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
            // 各行を処理
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

                    // マージイベントの位置を調整
                    var adjustedMergeEvents = moveResult.MergeEvents
                        .Select(e => new TileMergedEventArgs(
                            new Position(row, e.Position.Column),
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
                ? MoveResult.Success(
                    totalTilesMoved,
                    totalTilesMerged,
                    new Score(totalScoreGained),
                    allMergeEvents)
                : MoveResult.NoMove();

            Logger.LogDebug("Move left completed: {Result}", result);
            return Result<MoveResult>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing left move");
            return Result<MoveResult>.Failure($"Failed to execute left move: {ex.Message}");
        }
    }

    protected override ReadOnlySpan<Tile> GetLine(IGameBoard board, int lineIndex)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        var line = new Tile[GameConstants.BoardSize];
        for (var col = 0; col < GameConstants.BoardSize; col++)
        {
            line[col] = concreteBoard.Tiles[lineIndex, col];
        }
        return line.AsSpan();
    }

    protected override void SetLine(IGameBoard board, int lineIndex, ReadOnlySpan<Tile> line)
    {
        if (board is not Board concreteBoard)
            throw new ArgumentException("Board type not supported", nameof(board));

        for (var col = 0; col < GameConstants.BoardSize; col++)
        {
            concreteBoard.Tiles[lineIndex, col] = line[col];
        }
    }

    protected override IGameBoard CreateBoardCopy(IGameBoard original)
    {
        if (original is not Board originalBoard)
            throw new ArgumentException("Board type not supported", nameof(original));

        // 実際の実装では、Boardクラスにコピーコンストラクタが必要
        // ここでは簡略化のため、元のボードを返す
        return originalBoard;
    }
}
