namespace Blazor2048.Core;

/// <summary>
/// 移動コマンドのインターフェース
/// Command/Strategy パターンを適用した移動処理の抽象化
/// </summary>
public interface IMoveCommand
{
    /// <summary>
    /// 移動方向
    /// </summary>
    Direction Direction { get; }

    /// <summary>
    /// 移動が可能かどうかを事前チェック
    /// </summary>
    /// <param name="board">対象のゲームボード</param>
    /// <returns>移動可能な場合true</returns>
    bool CanExecute(IGameBoard board);

    /// <summary>
    /// 移動を実行
    /// </summary>
    /// <param name="board">対象のゲームボード</param>
    /// <returns>移動結果</returns>
    Result<MoveResult> Execute(IGameBoard board);

    /// <summary>
    /// 移動のプレビュー（実際には変更しない）
    /// </summary>
    /// <param name="board">対象のゲームボード</param>
    /// <returns>移動結果の予測</returns>
    Result<MoveResult> Preview(IGameBoard board);
}

/// <summary>
/// 移動コマンドファクトリー
/// </summary>
public interface IMoveCommandFactory
{
    /// <summary>
    /// 指定された方向の移動コマンドを作成
    /// </summary>
    /// <param name="direction">移動方向</param>
    /// <returns>移動コマンド</returns>
    IMoveCommand CreateCommand(Direction direction);

    /// <summary>
    /// 利用可能なすべての移動コマンドを取得
    /// </summary>
    /// <returns>移動コマンドのコレクション</returns>
    IEnumerable<IMoveCommand> GetAllCommands();
}
