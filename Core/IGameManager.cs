namespace Blazor2048.Core;

public interface IGameManager
{
    IGameBoard Board { get; }
    GameState State { get; }
    void Move(Direction direction);
    void Move(string direction);
    void Restart();

    event EventHandler<GameState>? StateChanged;
    event EventHandler<TileMergedEventArgs>? TileMerged;
}

