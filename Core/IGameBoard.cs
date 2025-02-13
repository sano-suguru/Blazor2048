namespace Blazor2048.Core;

public interface IGameBoard
{
    bool MoveTiles(Direction direction);
    void AddNewTile();
    bool IsGameOver();
    Tile[,] Tiles { get; }
}
