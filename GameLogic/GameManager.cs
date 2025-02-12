using Blazor2048.Core;

namespace Blazor2048.GameLogic;

public class GameManager(Board board)
{
    public Board Board { get; private set; } = board;
    public int Score { get; private set; }

    public void Move(string direction)
    {
        if (Enum.TryParse(direction, true, out Direction dir))
        {
            Move(dir);
        }
    }

    public void Move(Direction direction)
    {
        if (!Board.MoveTiles(direction)) return;
        Score += CalculateScore();
        if (IsGameOver()) Console.WriteLine("Game Over!");
    }

    public void Restart()
    {
        Board = new Board();
        Score = 0;
    }

    private int CalculateScore()
    {
        return Board.Tiles.Cast<Tile>().Sum(tile => tile.Value);
    }

    public bool IsGameOver()
    {
        return Board.Tiles.Cast<Tile>().All(tile => tile.Value != 0);
    }
}
