using Blazor2048.Core;

namespace Blazor2048.GameLogic;

public class GameManager(Board board)
{
    public Board Board { get; private set; } = board;
    public int Score => Board.Score;

    public void Move(Direction direction)
    {
        if (!Board.MoveTiles(direction)) return;
        if (IsGameOver()) Console.WriteLine("Game Over!");
    }

    public void Move(string direction)
    {
        if (Enum.TryParse(direction, true, out Direction dir))
        {
            Move(dir);
        }
    }

    public bool IsGameOver()
    {
        for (int i = 0; i < Board.Size; i++)
        {
            for (int j = 0; j < Board.Size; j++)
            {
                if (Board.Tiles[i, j] == 0) return false;
                if (i < Board.Size - 1 && Board.Tiles[i, j] == Board.Tiles[i + 1, j]) return false;
                if (j < Board.Size - 1 && Board.Tiles[i, j] == Board.Tiles[i, j + 1]) return false;
            }
        }
        return true;
    }

    public void Restart()
    {
        Board = new Board();
    }
}
