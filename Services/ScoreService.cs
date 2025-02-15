using Blazor2048.Core;

namespace Blazor2048.Services;

public class ScoreService(ILocalStorageService localStorage, ILogger<ScoreService> logger) : IScoreService
{
    private const string HighScoreKey = "highScore";
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly ILogger<ScoreService> _logger = logger;

    public event EventHandler<HighScore>? HighScoreUpdated;

    public async ValueTask<HighScore?> GetHighScoreAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<HighScore>(HighScoreKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving high score");
            return null;
        }
    }

    public async ValueTask SaveHighScoreAsync(Score score)
    {
        try
        {
            var currentHighScore = await GetHighScoreAsync();
            if (currentHighScore?.Value.Value >= score.Value)
            {
                return;
            }

            var newHighScore = HighScore.Create(score);
            await _localStorage.SetItemAsync(HighScoreKey, newHighScore);

            _logger.LogInformation("New high score saved: {Score}", score.Value);
            OnHighScoreUpdated(newHighScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving high score");
        }
    }

    protected virtual void OnHighScoreUpdated(HighScore highScore)
    {
        HighScoreUpdated?.Invoke(this, highScore);
    }
}