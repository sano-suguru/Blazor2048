namespace Blazor2048.Core;

public record GameState(Score CurrentScore, bool IsGameOver)
{
    public static GameState Initial => new(Score.Zero, false);

    public GameState WithScore(Score newScore) =>
        this with { CurrentScore = newScore };

    public GameState WithGameOver() =>
        this with { IsGameOver = true };
}
