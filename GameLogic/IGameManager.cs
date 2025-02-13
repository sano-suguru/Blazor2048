using Blazor2048.Core;

namespace Blazor2048.GameLogic;

public interface IGameManager
{
    IGameBoard Board { get; }
    Score CurrentScore { get; }
    void Move(Direction direction);
    void Move(string direction);
    void Restart();
    bool IsGameOver();
    event EventHandler<Score>? ScoreChanged;
    event EventHandler? GameOver;
}
