namespace Blazor2048.Core;

public readonly record struct Position(int Row, int Column)
{
    public static Position FromIndex(int index, int boardSize) =>
        new(index / boardSize, index % boardSize);

    public int ToIndex(int boardSize) => Row * boardSize + Column;

    public bool IsValid(int boardSize) =>
        Row >= 0 && Row < boardSize && Column >= 0 && Column < boardSize;
}
