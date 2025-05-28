using Microsoft.Extensions.Logging;

namespace Blazor2048.Core.Commands;

/// <summary>
/// 移動コマンドの抽象基底クラス
/// 共通処理とテンプレートメソッドパターンを提供
/// </summary>
public abstract class MoveCommandBase : IMoveCommand
{
    protected readonly ILogger Logger;

    public abstract Direction Direction { get; }

    protected MoveCommandBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual bool CanExecute(IGameBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);
        
        try
        {
            // 簡単なチェック：既存のBoard.CanMoveメソッドを使用
            return board.CanMove(Direction);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if move {Direction} can be executed", Direction);
            return false;
        }
    }

    public virtual Result<MoveResult> Execute(IGameBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        try
        {
            Logger.LogDebug("Executing move command for direction {Direction}", Direction);

            if (!CanExecute(board))
            {
                Logger.LogDebug("Move {Direction} cannot be executed", Direction);
                return Result<MoveResult>.Success(MoveResult.NoMove());
            }

            return ExecuteMove(board);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Failed to execute move {Direction}: {ex.Message}";
            Logger.LogError(ex, errorMessage);
            return Result<MoveResult>.Failure(errorMessage);
        }
    }

    public virtual Result<MoveResult> Preview(IGameBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        try
        {
            // ボードのコピーを作成してプレビュー実行
            var boardCopy = CreateBoardCopy(board);
            return ExecuteMove(boardCopy, isPreview: true);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Failed to preview move {Direction}: {ex.Message}";
            Logger.LogError(ex, errorMessage);
            return Result<MoveResult>.Failure(errorMessage);
        }
    }

    /// <summary>
    /// 実際の移動処理を実行する抽象メソッド
    /// </summary>
    protected abstract Result<MoveResult> ExecuteMove(IGameBoard board, bool isPreview = false);

    /// <summary>
    /// 指定されたラインを取得する抽象メソッド
    /// </summary>
    protected abstract ReadOnlySpan<Tile> GetLine(IGameBoard board, int lineIndex);

    /// <summary>
    /// 指定されたラインを設定する抽象メソッド
    /// </summary>
    protected abstract void SetLine(IGameBoard board, int lineIndex, ReadOnlySpan<Tile> line);

    /// <summary>
    /// ボードのコピーを作成
    /// </summary>
    protected virtual IGameBoard CreateBoardCopy(IGameBoard original)
    {
        // 実装は具体的なBoard型に依存するため、リフレクションまたは
        // ファクトリーパターンを使用して実装する必要があります
        // ここでは簡略化のため、元のボードをそのまま返します
        // 実際の実装では適切なコピー機能を実装する必要があります
        return original;
    }

    /// <summary>
    /// 行のマージ処理（最適化版）
    /// </summary>
    protected static MoveLineResult MergeLine(ReadOnlySpan<Tile> line)
    {
        var result = new Tile[line.Length];
        var mergeEvents = new List<TileMergedEventArgs>();
        var moved = false;
        var tilesMoved = 0;
        var tilesMerged = 0;
        var scoreGained = 0;

        // 空でないタイルを左詰めで配置
        var writeIndex = 0;
        for (var i = 0; i < line.Length; i++)
        {
            if (!line[i].IsEmpty)
            {
                result[writeIndex] = line[i];
                if (writeIndex != i) 
                {
                    moved = true;
                    tilesMoved++;
                }
                writeIndex++;
            }
        }

        // マージ処理
        for (var i = 0; i < writeIndex - 1; i++)
        {
            if (result[i].CanMergeWith(result[i + 1]))
            {
                var originalValue = result[i].Value;
                var mergedTile = result[i].MergeWith(result[i + 1]);
                
                result[i] = mergedTile;
                result[i + 1] = new Tile(0); // 空タイルに設定
                
                scoreGained += mergedTile.Value;
                tilesMerged++;
                moved = true;
                
                // マージイベントを記録（実際の位置は呼び出し元で計算）
                mergeEvents.Add(new TileMergedEventArgs(
                    new Position(0, i), // 位置は後で調整
                    originalValue,
                    mergedTile.Value
                ));
                
                i++; // 次のタイルをスキップ
            }
        }

        // マージ後の再配置
        if (tilesMerged > 0)
        {
            writeIndex = 0;
            for (var i = 0; i < result.Length; i++)
            {
                if (!result[i].IsEmpty)
                {
                    if (writeIndex != i)
                    {
                        result[writeIndex] = result[i];
                        result[i] = new Tile(0);
                    }
                    writeIndex++;
                }
            }
        }

        // 空のタイルで埋める
        for (var i = writeIndex; i < result.Length; i++)
        {
            result[i] = new Tile(0);
        }

        return new MoveLineResult(
            result.ToArray(),
            moved,
            tilesMoved,
            tilesMerged,
            new Score(scoreGained),
            mergeEvents
        );
    }

    /// <summary>
    /// 行のマージ結果
    /// </summary>
    protected readonly record struct MoveLineResult(
        Tile[] ResultLine,
        bool HasMoved,
        int TilesMoved,
        int TilesMerged,
        Score ScoreGained,
        List<TileMergedEventArgs> MergeEvents
    );
}
