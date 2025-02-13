using Blazor2048.Core;

namespace Blazor2048.GameLogic;

public class GameManager : IGameManager
{
    public IGameBoard Board { get; private set; }
    public Score CurrentScore { get; private set; }

    private readonly ILogger<GameManager> _logger;
    private readonly IRandomGenerator _randomGenerator;
    private readonly ILoggerFactory _loggerFactory;

    public event EventHandler<Score>? ScoreChanged;
    public event EventHandler? GameOver;

    public GameManager(
        ILogger<GameManager> logger,
        IRandomGenerator randomGenerator,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _randomGenerator = randomGenerator;
        _loggerFactory = loggerFactory;
        Board = new Board(_randomGenerator, _loggerFactory.CreateLogger<Board>());
        CurrentScore = Score.Zero;
    }

    public void Move(string direction)
    {
        if (Enum.TryParse(direction, true, out Direction dir))
        {
            Move(dir);
        }
        else
        {
            _logger.LogWarning("Invalid direction input: {Direction}", direction);
        }
    }

    public void Move(Direction direction)
    {
        try
        {
            _logger.LogInformation("Attempting move in direction: {Direction}", direction);

            if (!Board.MoveTiles(direction)) return;

            var newScore = CalculateScore();
            if (newScore.Value != CurrentScore.Value)
            {
                CurrentScore = newScore;
                OnScoreChanged();
            }

            if (IsGameOver())
            {
                OnGameOver();
            }
        }
        catch (GameException ex)
        {
            _logger.LogError(ex, "Error during move operation");
            throw;
        }
    }

    public void Restart()
    {
        _logger.LogInformation("Restarting game");
        Board = new Board(_randomGenerator, _loggerFactory.CreateLogger<Board>());
        CurrentScore = Score.Zero;
        OnScoreChanged();
    }

    private Score CalculateScore()
    {
        var score = Board.Tiles.Cast<Tile>().Sum(tile => tile.Value);
        return new Score(score);
    }

    public bool IsGameOver()
    {
        var isOver = Board.IsGameOver();
        if (isOver)
        {
            _logger.LogInformation("Game over detected. Final score: {Score}", CurrentScore.Value);
        }
        return isOver;
    }

    protected virtual void OnScoreChanged()
    {
        _logger.LogInformation("Score changed to: {Score}", CurrentScore.Value);
        ScoreChanged?.Invoke(this, CurrentScore);
    }

    protected virtual void OnGameOver()
    {
        _logger.LogInformation("Game over event triggered");
        GameOver?.Invoke(this, EventArgs.Empty);
    }
}
