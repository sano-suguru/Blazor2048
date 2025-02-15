using Blazor2048.Core;

namespace Blazor2048.GameLogic;

public interface IGameManager : IDisposable
{
    IGameBoard Board { get; }
    GameState State { get; }
    Task MoveAsync(Direction direction);
    Task MoveAsync(string direction);
    Task RestartAsync();
    event EventHandler<GameState>? StateChanged;
    event EventHandler<TileMergedEventArgs>? TileMerged;
}
