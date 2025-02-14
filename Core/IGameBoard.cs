namespace Blazor2048.Core;

public interface IGameBoard
{
    Tile[,] Tiles { get; }
    bool MoveTiles(Direction direction);
    void AddNewTile();
    bool IsGameOver();
    bool CanMove(Direction direction);
    event EventHandler<TileMergedEventArgs>? TileMerged;
}
