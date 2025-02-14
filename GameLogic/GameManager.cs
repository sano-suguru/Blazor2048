// GameLogic/GameManager.cs
using Blazor2048.Core;

namespace Blazor2048.GameLogic;

public class GameManager : IGameManager, IDisposable
{
    public IGameBoard Board { get; private set; }
    public GameState State { get; private set; }

    private readonly ILogger<GameManager> _logger;
    private readonly IRandomGenerator _randomGenerator;
    private readonly ILoggerFactory _loggerFactory;
    private bool _isDisposed;

    public event EventHandler<GameState>? StateChanged;
    public event EventHandler<TileMergedEventArgs>? TileMerged;

    public GameManager(
        ILogger<GameManager> logger,
        IRandomGenerator randomGenerator,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _randomGenerator = randomGenerator;
        _loggerFactory = loggerFactory;

        Board = CreateNewBoard();
        State = GameState.Initial;

        SubscribeToBoardEvents();
    }

    public void Move(string direction)
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(GameManager));

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
        if (_isDisposed) throw new ObjectDisposedException(nameof(GameManager));
        if (State.IsGameOver) return;

        try
        {
            _logger.LogInformation("Attempting move in direction: {Direction}", direction);

            if (!Board.MoveTiles(direction)) return;

            UpdateGameState();
        }
        catch (GameException ex)
        {
            _logger.LogError(ex, "Error during move operation");
            throw;
        }
    }

    public void Restart()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(GameManager));

        _logger.LogInformation("Restarting game");

        UnsubscribeFromBoardEvents();
        Board = CreateNewBoard();
        SubscribeToBoardEvents();

        State = GameState.Initial;
        OnStateChanged();
    }

    private Board CreateNewBoard() =>
        new(_randomGenerator, _loggerFactory.CreateLogger<Board>());

    private void UpdateGameState()
    {
        var newScore = CalculateScore();
        var isGameOver = Board.IsGameOver();

        var newState = State with
        {
            CurrentScore = newScore,
            IsGameOver = isGameOver
        };

        if (newState != State)
        {
            State = newState;
            OnStateChanged();
        }
    }

    private Score CalculateScore()
    {
        var score = Board.Tiles.Cast<Tile>().Sum(tile => tile.Value);
        return new Score(score);
    }

    private void HandleTileMerged(object? sender, TileMergedEventArgs e)
    {
        _logger.LogDebug("Tile merged event received: {OldValue} -> {NewValue} at {Position}",
            e.OldValue, e.NewValue, e.Position);

        OnTileMerged(e);
        UpdateGameState();
    }

    protected virtual void OnStateChanged()
    {
        _logger.LogInformation("Game state changed. Score: {Score}, GameOver: {IsGameOver}",
            State.CurrentScore.Value, State.IsGameOver);

        StateChanged?.Invoke(this, State);
    }

    protected virtual void OnTileMerged(TileMergedEventArgs e)
    {
        TileMerged?.Invoke(this, e);
    }

    private void SubscribeToBoardEvents()
    {
        if (Board is not null)
        {
            Board.TileMerged += HandleTileMerged;
        }
    }

    private void UnsubscribeFromBoardEvents()
    {
        if (Board is not null)
        {
            Board.TileMerged -= HandleTileMerged;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                UnsubscribeFromBoardEvents();
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
