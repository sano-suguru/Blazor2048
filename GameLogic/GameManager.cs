using Blazor2048.Core;
using Blazor2048.Core.Commands;
using Blazor2048.Services;

namespace Blazor2048.GameLogic;

public class GameManager : IGameManager, IDisposable
{
    public IGameBoard Board { get; private set; }
    public GameState State { get; private set; }

    private readonly ILogger<GameManager> _logger;
    private readonly IRandomGenerator _randomGenerator;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IScoreService _scoreService;
    private readonly IMoveCommandFactory _moveCommandFactory;
    private bool _isDisposed;

    public event EventHandler<GameState>? StateChanged;
    public event EventHandler<TileMergedEventArgs>? TileMerged;

    public GameManager(
        ILogger<GameManager> logger,
        IRandomGenerator randomGenerator,
        ILoggerFactory loggerFactory,
        IScoreService scoreService,
        IMoveCommandFactory moveCommandFactory)
    {
        _logger = logger;
        _randomGenerator = randomGenerator;
        _loggerFactory = loggerFactory;
        _scoreService = scoreService;
        _moveCommandFactory = moveCommandFactory;

        Board = CreateNewBoard();
        State = GameState.Initial;

        SubscribeToBoardEvents();
    }

    public async Task MoveAsync(string direction)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (Enum.TryParse(direction, true, out Direction dir))
        {
            await MoveAsync(dir);
        }
        else
        {
            _logger.LogWarning("Invalid direction input: {Direction}", direction);
        }
    }

    public async Task MoveAsync(Direction direction)
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(GameManager));
        if (State.IsGameOver) return;

        try
        {
            _logger.LogInformation("Attempting move in direction: {Direction}", direction);

            // 既存のBoard.MoveTilesメソッドを使用（一時的に元に戻す）
            if (!Board.MoveTiles(direction)) 
            {
                _logger.LogDebug("No movement occurred for direction {Direction}", direction);
                return;
            }

            await UpdateGameStateAsync();
            _logger.LogInformation("Move completed successfully for direction {Direction}", direction);
        }
        catch (GameException ex)
        {
            _logger.LogError(ex, "Error during move operation");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during move operation");
            throw new GameException($"Move operation failed: {ex.Message}", ex);
        }
    }

    public async Task RestartAsync()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(GameManager));

        _logger.LogInformation("Restarting game");

        UnsubscribeFromBoardEvents();
        Board = CreateNewBoard();
        SubscribeToBoardEvents();

        State = GameState.Initial;
        await UpdateGameStateAsync();
    }

    private Board CreateNewBoard() =>
        new(_randomGenerator, _loggerFactory.CreateLogger<Board>());

    private async Task UpdateGameStateAsync()
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

            if (isGameOver)
            {
                await _scoreService.SaveHighScoreAsync(newScore);
            }
        }
    }

    private Score CalculateScore()
    {
        var score = Board.Tiles.Cast<Tile>().Sum(tile => tile.Value);
        return new Score(score);
    }

    private async void HandleTileMerged(object? sender, TileMergedEventArgs e)
    {
        _logger.LogDebug("Tile merged event received: {OldValue} -> {NewValue} at {Position}",
            e.OldValue, e.NewValue, e.Position);

        OnTileMerged(e);
        await UpdateGameStateAsync();
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
