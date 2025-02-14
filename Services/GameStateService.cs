using Blazor2048.Core;

namespace Blazor2048.Services;

public class GameStateService(
    ILocalStorageService localStorage,
    ILogger<GameStateService> logger) : IGameStateService
{
    private const string GameStateKey = "gameState";
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly ILogger<GameStateService> _logger = logger;

    public async ValueTask<GameState?> LoadGameStateAsync()
    {
        try
        {
            var state = await _localStorage.GetItemAsync<GameState>(GameStateKey);
            if (state != null)
            {
                _logger.LogInformation("Loaded game state: Score={Score}, GameOver={IsGameOver}",
                    state.CurrentScore.Value, state.IsGameOver);
            }
            return state;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load game state");
            return null;
        }
    }

    public async ValueTask SaveGameStateAsync(GameState state)
    {
        try
        {
            await _localStorage.SetItemAsync(GameStateKey, state);
            _logger.LogInformation("Saved game state: Score={Score}, GameOver={IsGameOver}",
                state.CurrentScore.Value, state.IsGameOver);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save game state");
        }
    }

    public async ValueTask ClearGameStateAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync(GameStateKey);
            _logger.LogInformation("Cleared game state");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear game state");
        }
    }
}